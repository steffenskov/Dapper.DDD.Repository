using Dapper.Repository.Reflection;

namespace Dapper.Repository.Sql;

internal class SqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly SqlAggregateConfiguration<TAggregate> _configuration;
	private readonly string _schemaAndTable;

	public SqlQueryGenerator(SqlAggregateConfiguration<TAggregate> configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration.Schema);
		ArgumentNullException.ThrowIfNull(configuration.TableName);

		if (string.IsNullOrWhiteSpace(configuration.Schema))
			throw new ArgumentException("Schema cannot be null or whitespace.", nameof(configuration));

		if (string.IsNullOrWhiteSpace(configuration.TableName))
			throw new ArgumentException("Table name cannot be null or whitespace.", nameof(configuration));
		_configuration = configuration;
		_schemaAndTable = $"{EnsureSquareBrackets(configuration.Schema)}.{EnsureSquareBrackets(configuration.TableName)}";
	}

	public string GenerateDeleteQuery()
	{
		var whereClause = GenerateWhereClause();

		var outputColumns = GenerateColumnsList("deleted", _configuration.GetProperties());
		return $"DELETE FROM {_schemaAndTable} OUTPUT {outputColumns} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var idaggregateColumns = _configuration.GetIdaggregateProperties();
		var propertiesWithDefaultValues = _configuration.GetPropertiesWithDefaultConstraints();

		var columnsToInsert = _configuration.GetProperties()
									.Where(property => !idaggregateColumns.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		var outputColumns = GenerateColumnsList("inserted", _configuration.GetProperties());
		return $"INSERT INTO {_schemaAndTable} ({string.Join(", ", columnsToInsert.Select(column => AddSquareBrackets(column.Name)))}) OUTPUT {outputColumns} VALUES ({string.Join(", ", columnsToInsert.Select(column => $"@{column.Name}"))});";
	}

	public string GenerateGetAllQuery()
	{
		var columnsList = GenerateColumnsList(_schemaAndTable, _configuration.GetProperties());
		return $"SELECT {columnsList} FROM {_schemaAndTable};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var columnsList = GenerateColumnsList(_schemaAndTable, _configuration.GetProperties());

		return $"SELECT {columnsList} FROM {_schemaAndTable} WHERE {whereClause};";
	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputColumns = GenerateColumnsList("inserted", _configuration.GetProperties());

		return $"UPDATE {_schemaAndTable} SET {setClause} OUTPUT {outputColumns} WHERE {GenerateWhereClause()};";
	}

	private string GenerateSetClause()
	{
		var primaryKeys = _configuration.GetKeys();
		var columnsToSet = _configuration.GetProperties().Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		return string.Join(", ", columnsToSet.Select(column => $"{_schemaAndTable}.{AddSquareBrackets(column.Name)} = @{column.Name}"));
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _configuration.GetKeys();

		return string.Join(" AND ", primaryKeys.Select(column => $"{_schemaAndTable}.{AddSquareBrackets(column.Name)} = @{column.Name}"));
	}


	private string GenerateColumnsList(string tableName, IEnumerable<ExtendedPropertyInfo> properties)
	{
		tableName = EnsureSquareBrackets(tableName);

		return string.Join(", ", properties.Select(property => GenerateColumnClause(tableName, property)));
	}

	private static string GenerateColumnClause(string tableName, ExtendedPropertyInfo property)
	{
		return $"{tableName}.{AddSquareBrackets(property.Name)}";
	}

	private static string EnsureSquareBrackets(string name)
	{
		if (!name.StartsWith('['))
			return AddSquareBrackets(name);
		else
			return name;
	}

	private static string AddSquareBrackets(string name)
	{
		return $"[{name}]";
	}
}

