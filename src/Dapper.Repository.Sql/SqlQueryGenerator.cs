using Dapper.Repository.Reflection;

namespace Dapper.Repository.Sql;

internal class SqlQueryGenerator<TAggregate> : IQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly AggregateConfiguration<TAggregate> _configuration;
	private readonly string _schemaAndTable;

	public SqlQueryGenerator(AggregateConfiguration<TAggregate> configuration)
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

		var outputProperties = GeneratePropertyList("deleted", _configuration.GetProperties());
		return $"DELETE FROM {_schemaAndTable} OUTPUT {outputProperties} WHERE {whereClause};";
	}

	public string GenerateInsertQuery(TAggregate aggregate)
	{
		var identityProperties = _configuration.GetIdentityProperties();
		var propertiesWithDefaultValues = _configuration.GetPropertiesWithDefaultConstraints();

		var propertiesToInsert = _configuration.GetProperties()
									.Where(property => !identityProperties.Contains(property) && (!propertiesWithDefaultValues.Contains(property) || !property.HasDefaultValue(aggregate)))
									.ToList();

		var outputProperties = GeneratePropertyList("inserted", _configuration.GetProperties());
		return $"INSERT INTO {_schemaAndTable} ({string.Join(", ", propertiesToInsert.Select(property => AddSquareBrackets(property.Name)))}) OUTPUT {outputProperties} VALUES ({string.Join(", ", propertiesToInsert.Select(property => $"@{property.Name}"))});";
	}

	public string GenerateGetAllQuery()
	{
		var propertyList = GeneratePropertyList(_schemaAndTable, _configuration.GetProperties());
		return $"SELECT {propertyList} FROM {_schemaAndTable};";
	}

	public string GenerateGetQuery()
	{
		var whereClause = GenerateWhereClause();

		var propertyList = GeneratePropertyList(_schemaAndTable, _configuration.GetProperties());

		return $"SELECT {propertyList} FROM {_schemaAndTable} WHERE {whereClause};";
	}

	public string GenerateUpdateQuery()
	{
		var setClause = GenerateSetClause();

		if (string.IsNullOrEmpty(setClause))
		{
			throw new InvalidOperationException($"GenerateUpdateQuery for aggregate of type {typeof(TAggregate).FullName} failed as the type has no properties with a setter.");
		}

		var outputProperties = GeneratePropertyList("inserted", _configuration.GetProperties());

		return $"UPDATE {_schemaAndTable} SET {setClause} OUTPUT {outputProperties} WHERE {GenerateWhereClause()};";
	}

	private string GenerateSetClause()
	{
		var primaryKeys = _configuration.GetKeys();
		var propertiesToSet = _configuration.GetProperties().Where(property => !primaryKeys.Contains(property) && property.HasSetter);
		return string.Join(", ", propertiesToSet.Select(property => $"{_schemaAndTable}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
	}

	private string GenerateWhereClause()
	{
		var primaryKeys = _configuration.GetKeys();

		return string.Join(" AND ", primaryKeys.Select(property => $"{_schemaAndTable}.{AddSquareBrackets(property.Name)} = @{property.Name}"));
	}


	private string GeneratePropertyList(string tableName, IEnumerable<ExtendedPropertyInfo> properties)
	{
		tableName = EnsureSquareBrackets(tableName);

		return string.Join(", ", properties.Select(property => GeneratePropertyClause(tableName, property)));
	}

	private static string GeneratePropertyClause(string tableName, ExtendedPropertyInfo property)
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

