namespace Dapper.Repository.Sql;
public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(AggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return new SqlQueryGenerator<TAggregate>(configuration);
	}
}