using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.QueryGenerators;

public abstract class BaseQueryGenerator<TAggregate>
	where TAggregate : notnull
{
	private readonly IReadOnlyDictionary<string, string> _columnNameMap;

	protected BaseQueryGenerator(BaseAggregateConfiguration<TAggregate> configuration)
	{
		var readConfiguration = (IReadAggregateConfiguration<TAggregate>)configuration;
		_columnNameMap = readConfiguration.GetColumnNameMap();
	}

	protected virtual string GetColumnName(ExtendedPropertyInfo property, bool includeAsAlias)
	{
		return _columnNameMap.TryGetValue(property.Name, out var columnName)
			?  includeAsAlias ? GetColumnNameWithAlias(columnName, property.Name) : columnName
			: property.Name;
	}

	protected virtual string GetColumnNameWithAlias(string columnName, string aliasName)
	{
		return $"{columnName} AS {aliasName}";
	}
}