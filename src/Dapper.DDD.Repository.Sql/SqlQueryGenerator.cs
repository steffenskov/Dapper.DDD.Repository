using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Sql;

internal class SqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly string _schemaAndEntity;
	private readonly IReadOnlyExtendedPropertyInfoCollection _properties;
	private readonly IReadOnlyExtendedPropertyInfoCollection _identities;
	private readonly IReadOnlyExtendedPropertyInfoCollection _keys;
	private readonly IReadOnlyExtendedPropertyInfoCollection _defaultConstraints;

	public SqlQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration)
	{
		var readConfiguration = (IReadAggregateConfiguration<TAggregate>)configuration;
		ArgumentNullException.ThrowIfNull(configuration.Schema);
		ArgumentNullException.ThrowIfNull(readConfiguration.EntityName);

		if (string.IsNullOrWhiteSpace(configuration.Schema))
		{
			throw new ArgumentException("Schema cannot be null or whitespace.", nameof(configuration));
		}

		if (string.IsNullOrWhiteSpace(readConfiguration.EntityName))
		{
			throw new ArgumentException("Entity name cannot be null or whitespace.", nameof(configuration));
		}

		_schemaAndEntity = $"{EnsureSquareBrackets(configuration.Schema)}.{EnsureSquareBrackets(readConfiguration.EntityName)}";

		var properties = readConfiguration.GetProperties();
		var keys = new ExtendedPropertyInfoCollection(readConfiguration.GetKeys());
		var valueObjects = readConfiguration.GetValueObjects();
		foreach (var valueObject in valueObjects)
		{
			properties.Remove(valueObject);
			properties.AddRange(valueObject.GetPropertiesOrdered());
			if (keys.Contains(valueObject))
			{
				keys.Remove(valueObject);
				keys.AddRange(valueObject.GetPropertiesOrdered());
			}

		}
		_properties = properties;
		_identities = readConfiguration.GetIdentityProperties();
		_keys = keys;
		_defaultConstraints = readConfiguration.GetPropertiesWithDefaultConstraints();
	}

	public string GenerateDeleteQuery()
	{
		var whereClause = GenerateWhereClause();

		var outputProperties = GeneratePropertyList("deleted", _properties);
		return $"DELETE FROM {_schemaAndEntity} OUTPUT {outputProperties} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _identities;
		var propertiesWithDefaultValues = _defaultConstraints;

		var propertiesToInsert = _properties
									.Where(property => !identityProperties.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		var outputProperties = GeneratePropertyList("inserted", _properties);
		return $"INSERT INTO {_schemaAndEntity} ({string.Join(", ", propertiesToInsert.Select(property => AddSquareBrackets(property.Name)))}) OUTPUT {outputProperties} VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_schemaAndEntity, _properties);
		return $"SELECT {propertyList} FROM {_schemaAndEntity};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_schemaAndEntity, _properties);

		return $"SELECT {propertyList} FROM {_schemaAndEntity} WHERE {whereClause};";
	}

	public string GenerateUpdateQuery(TAggregate aggregate)
	{
		var setClause = GenerateSetClause(aggregate);

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputProperties = GeneratePropertyList("inserted", _properties);

		return $"UPDATE {_schemaAndEntity} SET {setClause} OUTPUT {outputProperties} WHERE {GenerateWhereClause()};";
	}

	private string GenerateSetClause(TAggregate aggregate)
	{
		var primaryKeys = _keys;
		var propertiesWithDefaultValues = _defaultConstraints;
		var propertiesToSet = _properties.Where(property => !primaryKeys.Contains(property) && property.HasSetter && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)));
		var result = string.Join(", ", propertiesToSet.Select(property => $"{_schemaAndEntity}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
		return result;
	}

	private string GenerateWhereClause()
	{
		return string.Join(" AND ", _keys.Select(property => $"{_schemaAndEntity}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
	}


	private string GeneratePropertyList(string tableName, IEnumerable<ExtendedPropertyInfo> properties, string prefix = "")
	{
		tableName = EnsureSquareBrackets(tableName);

		return string.Join(", ", properties.Select(property => GeneratePropertyClause(tableName, property, prefix)));
	}

	private string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property, string prefix = "")
	{
		return $"{tableName}.{AddSquareBrackets(prefix + property.Name)}";
	}

	private string EnsureSquareBrackets(string name)
	{
		return !name.StartsWith('[') ? AddSquareBrackets(name) : name;
	}

	private string AddSquareBrackets(string name)
	{
		return $"[{name}]";
	}
}

