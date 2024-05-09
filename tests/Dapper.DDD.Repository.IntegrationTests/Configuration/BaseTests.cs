namespace Dapper.DDD.Repository.IntegrationTests.Configuration;

[Collection(Consts.DatabaseCollection)]
public abstract class BaseTests
{
	protected BaseTests(IContainerFixture fixture)
	{
	}
}