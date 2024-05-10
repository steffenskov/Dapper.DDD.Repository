using Dapper.DDD.Repository.Exceptions;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<DapperRepositoryQueryException>
{
	public SinglePrimaryKeyAsyncTests(ContainerFixture containerFixture) : base(containerFixture)
	{
	}
}