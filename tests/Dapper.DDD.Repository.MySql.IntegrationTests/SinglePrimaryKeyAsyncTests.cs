namespace Dapper.DDD.Repository.MySql.IntegrationTests;
public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<MySqlException>, IClassFixture<Startup>
{
	public SinglePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
