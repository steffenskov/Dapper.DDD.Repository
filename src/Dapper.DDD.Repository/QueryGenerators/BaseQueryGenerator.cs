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

	protected  string GetColumnName(ExtendedPropertyInfo property)
	{
		return _columnNameMap.TryGetValue(property.Name, out var columnName)
			? columnName
			: property.Name;
	}
}