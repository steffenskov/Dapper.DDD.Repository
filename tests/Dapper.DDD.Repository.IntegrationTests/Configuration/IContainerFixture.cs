namespace Dapper.DDD.Repository.IntegrationTests.Configuration;

public interface IContainerFixture
{
	ServiceProvider Provider { get; }
}