namespace Dapper.DDD.Repository.UnitTests;
public class MockQueryGeneratorFactory : IQueryGeneratorFactory
{
	public MockQueryGeneratorFactory()
	{
	}

	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
	where TAggregate : notnull
	{
		return Mock.Of<IQueryGenerator<TAggregate>>();
	}
}