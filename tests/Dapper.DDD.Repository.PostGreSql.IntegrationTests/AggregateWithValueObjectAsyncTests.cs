namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class AggregateWithValueObjectAsyncTests : BaseAggregateWithValueObjectAsyncTests, IClassFixture<Startup>
{
	public AggregateWithValueObjectAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}