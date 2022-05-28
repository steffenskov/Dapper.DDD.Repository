namespace Dapper.Repository.Sql;
public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(AggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		if (configuration is not AggregateConfiguration<TAggregate> sqlConfiguration)
		{
			throw new ArgumentException($"Configuration must be of type {nameof(AggregateConfiguration<TAggregate>)}");
		}
		return new SqlQueryGenerator<TAggregate>(sqlConfiguration);
	}
}