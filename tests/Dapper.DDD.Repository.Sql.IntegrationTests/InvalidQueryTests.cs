namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class InvalidQueryTests : BaseInvalidQueryTests, IClassFixture<Startup>
{
	public InvalidQueryTests(Startup startup) : base(startup.Provider)
	{
	}
}
