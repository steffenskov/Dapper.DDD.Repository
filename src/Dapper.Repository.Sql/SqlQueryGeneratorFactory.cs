namespace Dapper.Repository.Sql;
public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		if (configuration is not BaseAggregateConfiguration<TAggregate> sqlConfiguration)
		{
			throw new ArgumentException($"Configuration must be of type {nameof(BaseAggregateConfiguration<TAggregate>)}");
		}
		return new SqlQueryGenerator<TAggregate>(sqlConfiguration);
	}
}