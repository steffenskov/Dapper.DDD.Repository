namespace Dapper.DDD.Repository.MySql;
public class MySqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return new MySqlQueryGenerator<TAggregate>(configuration);
	}
}