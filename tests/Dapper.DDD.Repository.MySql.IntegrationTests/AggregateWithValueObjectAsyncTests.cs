namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class AggregateWithValueObjectAsyncTests : BaseAggregateWithValueObjectAsyncTests, IClassFixture<Startup>
{
	public AggregateWithValueObjectAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}