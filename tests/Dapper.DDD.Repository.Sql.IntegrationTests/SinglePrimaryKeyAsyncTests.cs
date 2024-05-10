using Dapper.DDD.Repository.Exceptions;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class SinglePrimaryKeyAsyncTests : BaseSinglePrimaryKeyAsyncTests<DapperRepositoryQueryException>

{
	public SinglePrimaryKeyAsyncTests(ContainerFixture containerFixture) : base(containerFixture)
	{
	}
}