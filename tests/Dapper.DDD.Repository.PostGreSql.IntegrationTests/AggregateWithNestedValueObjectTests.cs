namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class AggregateWithNestedValueObjectTests : BaseAggregateWithNestedValueObjectTests, IClassFixture<Startup>
{
	public AggregateWithNestedValueObjectTests(Startup startup) : base(startup.Provider)
	{
	}
}