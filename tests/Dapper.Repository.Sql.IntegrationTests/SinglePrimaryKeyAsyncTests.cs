using Microsoft.Data.SqlClient;

namespace Dapper.Repository.Sql.IntegrationTests;
public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<SqlException>, IClassFixture<Startup>
{
	public SinglePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}