namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class AggregateWithValueObjectAsyncTests : BaseAggregateWithValueObjectAsyncTests<SqlException>, IClassFixture<Startup>
{
	public AggregateWithValueObjectAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
