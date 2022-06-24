using Dapper.Repository.Reflection;

namespace Dapper.Repository.MySql;

internal class MySqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadOnlyExtendedPropertyInfoCollection _properties;
	private readonly IReadOnlyExtendedPropertyInfoCollection _identities;
	private readonly IReadOnlyExtendedPropertyInfoCollection _keys;
	private readonly IReadOnlyExtendedPropertyInfoCollection _defaultConstraints;
	private readonly string _entityName;

	public MySqlQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration)
	{
		if (configuration.Schema is not null)
		{
			throw new ArgumentException("MySql doesn't support Schema.", nameof(configuration));
		}

		ArgumentNullException.ThrowIfNull(configuration.EntityName);
		if (string.IsNullOrWhiteSpace(configuration.EntityName))
		{
			throw new ArgumentException("Table name cannot be null or whitespace.", nameof(configuration));
		}

		_entityName = configuration.EntityName;

		var readConfiguration = (IReadAggregateConfiguration<TAggregate>)configuration;
		var properties = new ExtendedPropertyInfoCollection(readConfiguration.GetProperties());
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

		var outputProperties = GeneratePropertyList(_entityName, _properties);
		return $@"SELECT {outputProperties} FROM {_entityName} WHERE {whereClause};DELETE FROM {_entityName} WHERE {whereClause};";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_entityName, _properties);
		return $"SELECT {propertyList} FROM {_entityName};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_entityName, _properties);

		return $"SELECT {propertyList} FROM {_entityName} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _identities;
		var propertiesWithDefaultValues = _defaultConstraints;

		var propertiesToInsert = _properties
									.Where(property => !identityProperties.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		var selectStatement = "";
		if (identityProperties.Any())
		{
			if (identityProperties.Count > 1)
			{
				throw new InvalidOperationException("Cannot generate INSERT query for table with multiple identity properties");
			}
			var property = identityProperties.First();
			var propertyList = GeneratePropertyList(_entityName, _properties);
			selectStatement = $"SELECT {propertyList} FROM {_entityName} WHERE {_entityName}.{property.Name} = LAST_INSERT_ID();";
		}
		else
		{
			selectStatement = GenerateGetQuery();
		}
		return $@"INSERT INTO {_entityName} ({string.Join(", ", propertiesToInsert.Select(property => property.Name))}) VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});{selectStatement}";

	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateGetQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		_ = GeneratePropertyList("inserted", _properties);
		var selectStatement = GenerateGetQuery();
		return $@"UPDATE {_entityName} SET {setClause} WHERE {GenerateWhereClause()};{selectStatement}";
	}

	#region Helpers

	private string GenerateSetClause()
	{
		var primaryKeys = _keys;
		var propertiesToSet = _properties.Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		return string.Join(", ", propertiesToSet.Select(property => $"{property.Name} = @{property.Name}"));
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _keys;

		return string.Join(" AND ", primaryKeys.Select(property => $"{_entityName}.{property.Name} = @{property.Name}"));
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
