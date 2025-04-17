using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseProductListViewTests : BaseTests
{
	private readonly IProductListViewRepository _repository;

	protected BaseProductListViewTests(IContainerFixture fixture) : base(fixture)
	{
		_repository = fixture.Provider.GetRequiredService<IProductListViewRepository>();
	}


	[Fact]
	public async Task GetNullableIdByName_NameDoesNotExist_ReturnsNull()
	{
		// Act
		var productId = await _repository.GetProductIdByNameAsync("SomeNameThatDoesNotExistAtAll", TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(productId);
	}

	[Fact]
	public async Task GetNullableIdByName_NameExists_ReturnsValue()
	{
		// Arrange
		var all = await _repository.GetAllAsync(TestContext.Current.CancellationToken);
		var first = all.First();

		// Act

		var productId = await _repository.GetProductIdByNameAsync(first.ProductName, TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(first.ProductID, productId);
	}

	[Fact]
	public async Task GetAll_HaveRows_Valid()
	{
		// Act
		var all = await _repository.GetAllAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.True(all.Count() >= 2);
	}

	[Fact]
	public async Task Get_DoesNotExist_ReturnsNull()
	{
		// Act
		var aggregate = await _repository.GetAsync(int.MaxValue, TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(aggregate);
	}

	[Fact]
	public async Task Get_Exists_IsReturned()
	{
		// Arrange
		var all = await _repository.GetAllAsync(TestContext.Current.CancellationToken);

		// Act
		var aggregate = await _repository.GetAsync(all.First().ProductID, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(aggregate);
	}

	[Fact]
	public async Task GetByName_DoesNotExist_ReturnsNull()
	{
		// Act
		var aggregate = await _repository.GetByNameAsync("ProductNameThatDoesNotExist", TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(aggregate);
	}

	[Fact]
	public async Task GetByName_Exists_IsReturned()
	{
		// Arrange
		var all = await _repository.GetAllAsync(TestContext.Current.CancellationToken);

		// Act
		var aggregate = await _repository.GetByNameAsync(all.First().ProductName, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(aggregate);
	}
}