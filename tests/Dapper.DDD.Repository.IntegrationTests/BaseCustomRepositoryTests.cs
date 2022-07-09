using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseCustomRepositoryTests
{
	private readonly IServiceProvider _serviceProvider;

	public BaseCustomRepositoryTests(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	[Fact]
	public void GetService_RepositoryConfigured_RepositoryIsRetrieved()
	{
		// Act
		var repository = _serviceProvider.GetService<ICustomerRepository>();

		// Assert
		Assert.NotNull(repository);
		Assert.True(repository is ITableRepository<Customer, Guid>);
	}
}
