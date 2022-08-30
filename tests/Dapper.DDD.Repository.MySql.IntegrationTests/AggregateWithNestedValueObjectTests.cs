namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class AggregateWithNestedValueObjectTests : BaseAggregateWithNestedValueObjectTests, IClassFixture<Startup>
{
	public AggregateWithNestedValueObjectTests(Startup startup) : base(startup.Provider)
	{
	}
}
