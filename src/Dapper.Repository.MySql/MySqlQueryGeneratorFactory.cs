namespace Dapper.Repository.MySql;
public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(AggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return new MySqlQueryGenerator<TAggregate>(configuration);
	}
}