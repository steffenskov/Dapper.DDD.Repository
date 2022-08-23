using Dapper.DDD.Repository.Exceptions;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;
public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<DapperRepositoryQueryException>, IClassFixture<Startup>
{
	public SinglePrimaryKeyAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
