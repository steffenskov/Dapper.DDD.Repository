using Dapper.Repository.Reflection;

namespace Dapper.Repository.MySql;

internal class MySqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadAggregateConfiguration<TAggregate> _configuration;
	private readonly string _table;

	public MySqlQueryGenerator(AggregateConfiguration<TAggregate> configuration)
	{
		if (configuration.Schema is not null)
			throw new ArgumentException("MySql doesn't support Schema.", nameof(configuration));

		ArgumentNullException.ThrowIfNull(configuration.TableName);
		if (string.IsNullOrWhiteSpace(configuration.TableName))
			throw new ArgumentException("Table name cannot be null or whitespace.", nameof(configuration));

		_configuration = configuration;
		_table = configuration.TableName;
	}

	public string GenerateDeleteQuery()
	{
		var whereClause = GenerateWhereClause();

		var outputProperties = GeneratePropertyList(_table, _configuration.GetProperties());
		return $@"SELECT {outputProperties} FROM {_table} WHERE {whereClause};
DELETE FROM {_table} WHERE {whereClause};";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_table, _configuration.GetProperties());
		return $"SELECT {propertyList} FROM {_table};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_table, _configuration.GetProperties());

		return $"SELECT {propertyList} FROM {_table} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _configuration.GetIdentityProperties();
		var propertiesWithDefaultValues = _configuration.GetPropertiesWithDefaultConstraints();

		var propertiesToInsert = _configuration.GetProperties()
									.Where(property => !identityProperties.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		string selectStatement = "";
		if (identityProperties.Any())
		{
			if (identityProperties.Count > 1)
			{
				throw new InvalidOperationException("Cannot generate INSERT query for table with multiple identity properties");
			}
			var property = identityProperties.First();
			var propertyList = GeneratePropertyList(_table, _configuration.GetProperties());
			selectStatement = $"SELECT {propertyList} FROM {_table} WHERE {_table}.{property.Name} = LAST_INSERT_ID();";
		}
		else
		{
			selectStatement = GenerateGetQuery();
		}
		return $@"INSERT INTO {_table} ({string.Join(", ", propertiesToInsert.Select(property => property.Name))}) VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});
{selectStatement}";

	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateGetQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputProperties = GeneratePropertyList("inserted", _configuration.GetProperties());
		var selectStatement = GenerateGetQuery();
		return $@"UPDATE {_table} SET {setClause} WHERE {GenerateWhereClause()};
{selectStatement}";
	}

	#region Helpers

	private string GenerateSetClause()
	{
		var primaryKeys = _configuration.GetKeys();
		var propertiesToSet = _configuration.GetProperties().Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		return string.Join(", ", propertiesToSet.Select(property => $"{property.Name} = @{property.Name}"));
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _configuration.GetKeys();

		return string.Join(" AND ", primaryKeys.Select(property => $"{_table}.{property.Name} = @{property.Name}"));
	}

	private string GeneratePropertyList(string tableName, IEnumerable<ExtendedPropertyInfo> propertiess)
	{
		return string.Join(", ", propertiess.Select(property => GeneratePropertyClause(tableName, property)));
	}

	private static string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property)
	{
		return $"{tableName}.{property.Name}";
	}
	#endregion
}
