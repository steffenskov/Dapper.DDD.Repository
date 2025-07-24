using Dapper.DDD.Repository.QueryGenerators;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.PostGreSql;

internal class PostGreSqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadOnlyExtendedPropertyInfoCollection _defaultConstraints;
	private readonly IReadOnlyExtendedPropertyInfoCollection _identities;
	private readonly IReadOnlyExtendedPropertyInfoCollection _keys;
	private readonly IReadOnlyExtendedPropertyInfoCollection _properties;
	private readonly string _schemaAndEntity;

	public PostGreSqlQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration)
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

		_schemaAndEntity = $"{configuration.Schema}.{readConfiguration.EntityName}";

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

		var outputProperties = GeneratePropertyList(_schemaAndEntity);
		return $"DELETE FROM {_schemaAndEntity} WHERE {whereClause} RETURNING {outputProperties};";
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

		var outputProperties = GeneratePropertyList(_schemaAndEntity);
		return
			$"INSERT INTO {_schemaAndEntity} ({string.Join(", ", propertiesToInsert.Select(property => property.Name))}) VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))}) RETURNING {outputProperties};";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_schemaAndEntity);
		return $"SELECT {propertyList} FROM {_schemaAndEntity};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_schemaAndEntity);

		return $"SELECT {propertyList} FROM {_schemaAndEntity} WHERE {whereClause};";
	}

	public string GenerateUpdateQuery(TAggregate aggregate)
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException(
				$"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputProperties = GeneratePropertyList(_schemaAndEntity);

		return
			$"UPDATE {_schemaAndEntity} SET {setClause} WHERE {GenerateWhereClause()} RETURNING {outputProperties};";
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

		var setClause = GenerateSetClause();
		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException("PostGreSql does not support Upsert on tables with no updatable columns.");
		}


		var returningIndex = insertQuery.IndexOf(" RETURNING ");
		insertQuery = insertQuery.Remove(returningIndex);
		var primaryKeys = string.Join(", ", _keys.Select(prop => prop.Name));

		var conflictResolution = $"ON CONFLICT ({primaryKeys}) DO UPDATE";
		var outputProperties = GeneratePropertyList(_schemaAndEntity);
		var updateQuery = $"SET {setClause} WHERE {GenerateWhereClause()} RETURNING {outputProperties};";

		return $"{insertQuery} {conflictResolution} {updateQuery}";
	}

	public string GeneratePropertyList(string tableName)
	{
		return string.Join(", ", _properties.Select(property => GeneratePropertyClause(tableName, property)));
	}

	private string GenerateSetClause()
	{
		var primaryKeys = _keys;
		var propertiesToSet = _properties.Where(property =>
			!primaryKeys.Contains(property) && property.HasSetter);
		var result = string.Join(", ",
			propertiesToSet.Select(property =>
				$"{property.Name} = @{property.Name}"));
		return result;
	}

	private string GenerateWhereClause()
	{
		return string.Join(" AND ",
			_keys.Select(property => $"{_schemaAndEntity}.{property.Name} = @{property.Name}"));
	}

	private static string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property)
	{
		return $"{tableName}.{property.Name}";
	}
}