namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class AggregateWithNestedValueObjectTests : BaseAggregateWithNestedValueObjectTests, IClassFixture<Startup>
{
	public AggregateWithNestedValueObjectTests(Startup startup) : base(startup.Provider)
	{
	}
}
