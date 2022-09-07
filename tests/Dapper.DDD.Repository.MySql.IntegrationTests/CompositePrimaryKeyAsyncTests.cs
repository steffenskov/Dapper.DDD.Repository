namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class CompositePrimaryKeyAsyncTests : BaseCompositePrimaryKeyAsyncTests, IClassFixture<Startup>
{
	public CompositePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}