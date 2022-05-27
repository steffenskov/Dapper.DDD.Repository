namespace Dapper.Repository.Sql.IntegrationTests;
public class CompositePrimaryKeyAsyncTests : BaseCompositePrimaryKeyAsyncTests, IClassFixture<Startup>
{
	public CompositePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}