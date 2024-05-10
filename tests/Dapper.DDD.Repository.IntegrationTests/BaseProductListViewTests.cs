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
		var productId = await _repository.GetProductIdByNameAsync("SomeNameThatDoesNotExistAtAll");
		
		// Assert
		Assert.Null(productId);
	}
	
	[Fact]
	public async Task GetNullableIdByName_NameExists_ReturnsValue()
	{
		// Arrange
		var all = await _repository.GetAllAsync();
		var first = all.First();
		
		// Act
		
		var productId = await _repository.GetProductIdByNameAsync(first.ProductName);
		
		// Assert
		Assert.Equal(first.ProductID, productId);
	}
	
	[Fact]
	public async Task GetAll_HaveRows_Valid()
	{
		// Act
		var all = await _repository.GetAllAsync();
		
		// Assert
		Assert.True(all.Count() >= 2);
	}
	
	[Fact]
	public async Task Get_DoesNotExist_ReturnsNull()
	{
		// Act
		var aggregate = await _repository.GetAsync(int.MaxValue);
		
		// Assert
		Assert.Null(aggregate);
	}
	
	[Fact]
	public async Task Get_Exists_IsReturned()
	{
		// Arrange
		var all = await _repository.GetAllAsync();
		
		// Act
		var aggregate = await _repository.GetAsync(all.First().ProductID);
		
		// Assert
		Assert.NotNull(aggregate);
	}
	
	[Fact]
	public async Task GetByName_DoesNotExist_ReturnsNull()
	{
		// Act
		var aggregate = await _repository.GetByNameAsync("ProductNameThatDoesNotExist");
		
		// Assert
		Assert.Null(aggregate);
	}
	
	[Fact]
	public async Task GetByName_Exists_IsReturned()
	{
		// Arrange
		var all = await _repository.GetAllAsync();
		
		// Act
		var aggregate = await _repository.GetByNameAsync(all.First().ProductName);
		
		// Assert
		Assert.NotNull(aggregate);
	}
}