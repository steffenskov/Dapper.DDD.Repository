namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class InvalidQueryTests : BaseInvalidQueryTests, IClassFixture<Startup>
{
	public InvalidQueryTests(Startup startup) : base(startup.Provider)
	{
	}
}
