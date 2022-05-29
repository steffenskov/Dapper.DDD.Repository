using Dapper.Repository.Reflection;

namespace Dapper.Repository.Sql;

internal class SqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly string _schemaAndTable;
	private IReadOnlyList<ExtendedPropertyInfo> _properties;
	private IReadOnlyList<ExtendedPropertyInfo> _identities;
	private IReadOnlyList<ExtendedPropertyInfo> _keys;
	private IReadOnlyList<ExtendedPropertyInfo> _defaultConstraints;

	public SqlQueryGenerator(AggregateConfiguration<TAggregate> configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration.Schema);
		ArgumentNullException.ThrowIfNull(configuration.TableName);

		if (string.IsNullOrWhiteSpace(configuration.Schema))
			throw new ArgumentException("Schema cannot be null or whitespace.", nameof(configuration));

		if (string.IsNullOrWhiteSpace(configuration.TableName))
			throw new ArgumentException("Table name cannot be null or whitespace.", nameof(configuration));
		_schemaAndTable = $"{EnsureSquareBrackets(configuration.Schema)}.{EnsureSquareBrackets(configuration.TableName)}";

		var readConfiguration = (IReadAggregateConfiguration<TAggregate>)configuration;
		var properties = readConfiguration.GetProperties().ToList();
		var valueObjects = readConfiguration.GetValueObjects();
		foreach (var valueObject in valueObjects)
		{
			properties.Remove(valueObject);
			properties.AddRange(valueObject.GetProperties());
		}
		_properties = properties;
		_identities = readConfiguration.GetIdentityProperties();
		_keys = readConfiguration.GetKeys();
		_defaultConstraints = readConfiguration.GetPropertiesWithDefaultConstraints();
	}

	public string GenerateDeleteQuery()
	{
		var whereClause = GenerateWhereClause();

		var outputProperties = GeneratePropertyList("deleted", _properties);
		return $"DELETE FROM {_schemaAndTable} OUTPUT {outputProperties} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _identities;
		var propertiesWithDefaultValues = _defaultConstraints;

		var propertiesToInsert = _properties
									.Where(property => !identityProperties.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		var outputProperties = GeneratePropertyList("inserted", _properties);
		return $"INSERT INTO {_schemaAndTable} ({string.Join(", ", propertiesToInsert.Select(property => AddSquareBrackets(property.Name)))}) OUTPUT {outputProperties} VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_schemaAndTable, _properties);
		return $"SELECT {propertyList} FROM {_schemaAndTable};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_schemaAndTable, _properties);

		return $"SELECT {propertyList} FROM {_schemaAndTable} WHERE {whereClause};";
	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputProperties = GeneratePropertyList("inserted", _properties);

		return $"UPDATE {_schemaAndTable} SET {setClause} OUTPUT {outputProperties} WHERE {GenerateWhereClause()};";
	}

	private string GenerateSetClause()
	{
		var primaryKeys = _keys;
		var propertiesToSet = _properties.Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		var result = string.Join(", ", propertiesToSet.Select(property => $"{_schemaAndTable}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
		return result;
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _keys;

		return string.Join(" AND ", primaryKeys.Select(property => $"{_schemaAndTable}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
	}


	private string GeneratePropertyList(string tableName, IEnumerable<ExtendedPropertyInfo> properties, string prefix = "")
	{
		tableName = EnsureSquareBrackets(tableName);

		return string.Join(", ", properties.Select(property => GeneratePropertyClause(tableName, property, prefix)));
	}

	private static IOrderedEnumerable<ExtendedPropertyInfo> GetChildProperties(ExtendedPropertyInfo property)
	{
		return TypePropertiesCache.GetProperties(property.Type).Values.OrderBy(prop => prop.Name);
	}

	private string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property, string prefix = "")
	{
		return $"{tableName}.{AddSquareBrackets(prefix + property.Name)}";
	}

	private string EnsureSquareBrackets(string name)
	{
		if (!name.StartsWith('['))
			return AddSquareBrackets(name);
		else
			return name;
	}

	private string AddSquareBrackets(string name)
	{
		return $"[{name}]";
	}
}

