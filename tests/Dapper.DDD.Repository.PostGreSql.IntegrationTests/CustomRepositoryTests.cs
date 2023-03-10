namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class CustomRepositoryTests : BaseCustomRepositoryTests, IClassFixture<Startup>
{
	public CustomRepositoryTests(Startup startup) : base(startup.Provider)
	{
	}
}