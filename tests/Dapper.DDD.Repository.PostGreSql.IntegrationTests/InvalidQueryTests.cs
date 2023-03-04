namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class InvalidQueryTests : BaseInvalidQueryTests, IClassFixture<Startup>
{
	public InvalidQueryTests(Startup startup) : base(startup.Provider)
	{
	}
}
