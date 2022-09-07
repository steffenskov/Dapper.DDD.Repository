namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class CustomRepositoryTests : BaseCustomRepositoryTests, IClassFixture<Startup>
{
	public CustomRepositoryTests(Startup startup) : base(startup.Provider)
	{
	}
}