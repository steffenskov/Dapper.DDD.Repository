namespace Dapper.Repository.MySql.IntegrationTests;

public class AggregateWithValueObjectAsyncTests : BaseAggregateWithValueObjectAsyncTests<MySqlException>, IClassFixture<Startup>
{
	public AggregateWithValueObjectAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
