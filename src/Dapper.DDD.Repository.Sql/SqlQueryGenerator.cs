using Dapper.DDD.Repository.QueryGenerators;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Sql;

internal class SqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadOnlyExtendedPropertyInfoCollection _defaultConstraints;
	private readonly bool _hasTriggers;
	private readonly IReadOnlyExtendedPropertyInfoCollection _identities;
	private readonly IReadOnlyExtendedPropertyInfoCollection _keys;
	private readonly IReadOnlyExtendedPropertyInfoCollection _properties;
	private readonly string _schemaAndEntity;
	private readonly IList<Predicate<Type>> _serializeColumnTypePredicates;

	public SqlQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration,
		IList<Predicate<Type>>? serializeColumnTypePredicates = null)
	{
		_serializeColumnTypePredicates = serializeColumnTypePredicates ?? Array.Empty<Predicate<Type>>();
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

		_schemaAndEntity =
			$"{EnsureSquareBrackets(configuration.Schema)}.{EnsureSquareBrackets(readConfiguration.EntityName)}";

		if (configuration is TableAggregateConfiguration<TAggregate> tableConfiguration)
		{
			_hasTriggers = tableConfiguration.HasTriggers;
		}

		var properties = readConfiguration.GetProperties();
		var keys = new ExtendedPropertyInfoCollection(readConfiguration.GetKeys());
		var valueObjects = readConfiguration.GetValueObjects();
		foreach (var valueObject in valueObjects)
		{
			var valueObjectProperties = valueObject.GetFlattenedNonComputedPropertiesOrdered(configuration);
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

		if (_hasTriggers)
		{
			return $"DELETE FROM {_schemaAndEntity} WHERE {whereClause};";
		}

		var outputProperties = GeneratePropertyList("deleted");
		return $"DELETE FROM {_schemaAndEntity} OUTPUT {outputProperties} WHERE {whereClause};";
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

		if (!_hasTriggers)
		{
			var outputProperties = GeneratePropertyList("inserted");
			return
				$"INSERT INTO {_schemaAndEntity} ({string.Join(", ", propertiesToInsert.Select(property => AddSquareBrackets(property.Name)))}) OUTPUT {outputProperties} VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});";
		}

		var propertyList = GeneratePropertyList(_schemaAndEntity);
		string whereClause;
		if (_identities.Any())
		{
			if (_identities.Count > 1)
			{
				throw new InvalidOperationException("Cannot generate INSERT query for tables with triggers and more than 1 identity column");
			}

			var identityProperty = _identities[0];
			whereClause = $"{_schemaAndEntity}.{AddSquareBrackets(identityProperty.Name)} = SCOPE_IDENTITY()";
		}
		else
		{
			whereClause = GenerateWhereClause();
		}

		return
			$"INSERT INTO {_schemaAndEntity} ({string.Join(", ", propertiesToInsert.Select(property => AddSquareBrackets(property.Name)))}) VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))}); SELECT {propertyList} FROM {_schemaAndEntity} WHERE {whereClause};";
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
			throw new InvalidOperationException($"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		if (_hasTriggers)
		{
			return $"UPDATE {_schemaAndEntity} SET {setClause} WHERE {GenerateWhereClause()};";
		}

		var outputProperties = GeneratePropertyList("inserted");

		return $"UPDATE {_schemaAndEntity} SET {setClause} OUTPUT {outputProperties} WHERE {GenerateWhereClause()};";
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

		var whereClause = GenerateWhereClause();
		var setClause = GenerateSetClause();
		var updateQuery = string.IsNullOrEmpty(setClause)
			? GenerateGetQuery() // Use select query instead of update, as nothing can be updated but we still expect the aggregate to be returned
			: GenerateUpdateQuery(aggregate);

		return $@"IF EXISTS (SELECT TOP 1 1 FROM {_schemaAndEntity} WHERE {whereClause})
BEGIN
{updateQuery}
END
ELSE
BEGIN
{insertQuery}
END";
	}

	public string GeneratePropertyList(string tableName)
	{
		tableName = EnsureSquareBrackets(tableName);

		return string.Join(", ", _properties.Select(property => GeneratePropertyClause(tableName, property)));
	}

	private string GenerateSetClause()
	{
		var primaryKeys = _keys;
		var propertiesToSet = _properties.Where(property =>
			!primaryKeys.Contains(property) && property.HasSetter);
		var result = string.Join(", ",
			propertiesToSet.Select(property =>
				$"{_schemaAndEntity}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
		return result;
	}

	private string GenerateWhereClause()
	{
		return string.Join(" AND ",
			_keys.Select(property => $"{_schemaAndEntity}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
	}

	private string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property)
	{
		var shouldSerialize = ShouldSerializeColumnType(property.Type);
		var result = $"{tableName}.{AddSquareBrackets(property.Name)}";
		return shouldSerialize
			? $"({result}).Serialize() AS [{property.Name}]"
			: result;
	}

	private bool ShouldSerializeColumnType(Type type)
	{
		return _serializeColumnTypePredicates.Any(predicate => predicate(type));
	}

	private static string EnsureSquareBrackets(string name)
	{
		return !name.StartsWith('[')
			? AddSquareBrackets(name)
			: name;
	}

	private static string AddSquareBrackets(string name)
	{
		return $"[{name}]";
	}
}