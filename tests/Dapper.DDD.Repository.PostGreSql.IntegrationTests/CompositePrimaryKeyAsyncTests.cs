namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class CompositePrimaryKeyAsyncTests : BaseCompositePrimaryKeyAsyncTests, IClassFixture<Startup>
{
	public CompositePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}