using Dapper.Repository.Reflection;

namespace Dapper.Repository.MySql;

internal class MySqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly AggregateConfiguration<TAggregate> _configuration;
	private readonly string _table;

	public MySqlQueryGenerator(AggregateConfiguration<TAggregate> configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration.TableName);
		if (string.IsNullOrWhiteSpace(configuration.TableName))
			throw new ArgumentException("Table name cannot be null or whitespace.", nameof(configuration));

		_configuration = configuration;
		_table = configuration.TableName;
	}

	public string GenerateDeleteQuery()
	{
		var whereClause = GenerateWhereClause();

		var outputColumns = GenerateColumnsList(_table, _configuration.GetProperties());
		return $@"SELECT {outputColumns} FROM {_table} WHERE {whereClause};
DELETE FROM {_table} WHERE {whereClause};";
	}

	public string GenerateGetAllQuery()
	{
		var columnsList = GenerateColumnsList(_table, _configuration.GetProperties());
		return $"SELECT {columnsList} FROM {_table};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var columnsList = GenerateColumnsList(_table, _configuration.GetProperties());

		return $"SELECT {columnsList} FROM {_table} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var idaggregateColumns = _configuration.GetIdaggregateProperties();
		var propertiesWithDefaultValues = _configuration.GetPropertiesWithDefaultConstraints();

		var columnsToInsert = _configuration.GetProperties()
									.Where(property => !idaggregateColumns.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		string selectStatement = "";
		if (idaggregateColumns.Any())
		{
			var column = idaggregateColumns.SingleOrDefault();
			if (column is null)
			{
				throw new InvalidOperationException("Cannot generate INSERT query for table with multiple idaggregate columns");
			}
			var columnsList = GenerateColumnsList(_table, _configuration.GetProperties());
			selectStatement = $"SELECT {columnsList} FROM {_table} WHERE {_table}.{column.Name} = LAST_INSERT_ID();";
		}
		else
		{
			selectStatement = GenerateGetQuery();
		}
		return $@"INSERT INTO {_table} ({string.Join(", ", columnsToInsert.Select(column => column.Name))}) VALUES ({string.Join(", ", columnsToInsert.Select(column => $"@{column.Name}"))});
{selectStatement}";

	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateGetQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputColumns = GenerateColumnsList("inserted", _configuration.GetProperties());
		var selectStatement = GenerateGetQuery();
		return $@"UPDATE {_table} SET {setClause} WHERE {GenerateWhereClause()};
{selectStatement}";
	}

	#region Helpers

	private string GenerateSetClause()
	{
		var primaryKeys = _configuration.GetKeys();
		var columnsToSet = _configuration.GetProperties().Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		return string.Join(", ", columnsToSet.Select(column => $"{column.Name} = @{column.Name}"));
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _configuration.GetKeys();

		return string.Join(" AND ", primaryKeys.Select(column => $"{_table}.{column.Name} = @{column.Name}"));
	}

	private string GenerateColumnsList(string tableName, IEnumerable<ExtendedPropertyInfo> columns)
	{
		return string.Join(", ", columns.Select(column => GenerateColumnClause(tableName, column)));
	}

	private static string GenerateColumnClause(string tableName, ExtendedPropertyInfo column)
	{
		return $"{tableName}.{column.Name}";
	}
	#endregion
}
