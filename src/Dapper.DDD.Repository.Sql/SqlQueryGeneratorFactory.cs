namespace Dapper.DDD.Repository.Sql;
public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return configuration is not BaseAggregateConfiguration<TAggregate> sqlConfiguration
			? throw new ArgumentException($"Configuration must be of type {nameof(BaseAggregateConfiguration<TAggregate>)}")
			: (IQueryGenerator<TAggregate>)new SqlQueryGenerator<TAggregate>(sqlConfiguration);
	}
}