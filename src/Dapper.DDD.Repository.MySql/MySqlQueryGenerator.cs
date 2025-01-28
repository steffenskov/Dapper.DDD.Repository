using Dapper.DDD.Repository.QueryGenerators;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.MySql;

internal class MySqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadOnlyExtendedPropertyInfoCollection _defaultConstraints;
	private readonly string _entityName;
	private readonly IReadOnlyExtendedPropertyInfoCollection _identities;
	private readonly IReadOnlyExtendedPropertyInfoCollection _keys;
	private readonly IReadOnlyExtendedPropertyInfoCollection _properties;

	public MySqlQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration)
	{
		var readConfiguration = (IReadAggregateConfiguration<TAggregate>)configuration;
		if (configuration.Schema is not null)
		{
			throw new ArgumentException("MySql doesn't support Schema.", nameof(configuration));
		}

		ArgumentNullException.ThrowIfNull(readConfiguration.EntityName);
		if (string.IsNullOrWhiteSpace(readConfiguration.EntityName))
		{
			throw new ArgumentException("Entity name cannot be null or whitespace.", nameof(configuration));
		}

		_entityName = readConfiguration.EntityName;

		var properties = readConfiguration.GetProperties();
		var keys = new ExtendedPropertyInfoCollection(readConfiguration.GetKeys());
		var valueObjects = readConfiguration.GetValueObjects();
		foreach (var valueObject in valueObjects)
		{
			var valueObjectProperties = valueObject.GetFlattenedPropertiesOrdered(configuration);
			properties.Remove(valueObject);
			properties.AddRange(valueObjectProperties);
			if (keys.Contains(valueObject))
			{
				keys.Remove(valueObject);
				keys.AddRange(valueObjectProperties);
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

		var outputProperties = GeneratePropertyList(_entityName);
		return
			$@"SELECT {outputProperties} FROM {_entityName} WHERE {whereClause};DELETE FROM {_entityName} WHERE {whereClause};";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_entityName);
		return $"SELECT {propertyList} FROM {_entityName};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_entityName);

		return $"SELECT {propertyList} FROM {_entityName} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _identities;
		var propertiesWithDefaultValues = _defaultConstraints;

		var propertiesToInsert = _properties
			.Where(property => !identityProperties.Contains(property) &&
			                   (!propertiesWithDefaultValues.Contains(property) ||
			                    !property.HasDefaultValue(aggregate)))
			.ToList();

		var selectStatement = "";
		if (identityProperties.Any())
		{
			if (identityProperties.Count > 1)
			{
				throw new InvalidOperationException(
					"Cannot generate INSERT query for table with multiple identity properties");
			}

			var property = identityProperties.First();
			var propertyList = GeneratePropertyList(_entityName);
			selectStatement =
				$"SELECT {propertyList} FROM {_entityName} WHERE {_entityName}.{property.Name} = LAST_INSERT_ID();";
		}
		else
		{
			selectStatement = GenerateGetQuery();
		}

		return
			$@"INSERT INTO {_entityName} ({string.Join(", ", propertiesToInsert.Select(property => property.Name))}) VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});{selectStatement}";
	}

	public string GenerateUpdateQuery(TAggregate aggregate)
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException(
				$"GenerateGetQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var selectStatement = GenerateGetQuery();
		return $@"UPDATE {_entityName} SET {setClause} WHERE {GenerateWhereClause()};{selectStatement}";
	}

	public string GenerateUpsertQuery(TAggregate aggregate)
	{
		var insertQuery = GenerateInsertQuery(aggregate);

		if (_identities.Any())
		{
			return _identities.Any(prop => !prop.HasDefaultValue(aggregate))
				? GenerateUpdateQuery(aggregate) // One or more identities have a specified value => do an update 
				: insertQuery; // All identities are default => do an insert
		}

		var semicolonIndex = insertQuery.IndexOf(';');
		var insertPart = insertQuery[..semicolonIndex];
		var selectQuery = GenerateGetQuery();
		var setClause = GenerateSetClause();
		var onDuplicateClause = string.IsNullOrWhiteSpace(setClause)
			? ""
			: $" ON DUPLICATE KEY UPDATE {setClause}";
		return $"{insertPart}{onDuplicateClause};{selectQuery}";
	}

	#region Helpers
	private string GenerateSetClause()
	{
		var primaryKeys = _keys;
		var propertiesToSet = _properties.Where(property =>
			!primaryKeys.Contains(property) && property.HasSetter );
		return string.Join(", ", propertiesToSet.Select(property => $"{property.Name} = @{property.Name}"));
	}

	private string GenerateWhereClause()
	{
		return string.Join(" AND ",
			_keys.Select(property => $"{_entityName}.{property.Name} = @{property.Name}"));
	}

	public string GeneratePropertyList(string tableName)
	{
		return string.Join(", ", _properties.Select(property => GeneratePropertyClause(tableName, property)));
	}

	private static string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property)
	{
		return $"{tableName}.{property.Name}";
	}
	#endregion
}