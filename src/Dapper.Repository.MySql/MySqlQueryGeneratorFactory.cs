namespace Dapper.Repository.MySql;
public class MySqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(AggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return new MySqlQueryGenerator<TAggregate>(configuration);
	}
}