namespace Dapper.DDD.Repository.Sql.IntegrationTests;
public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<SqlException>, IClassFixture<Startup>
{
	public SinglePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
