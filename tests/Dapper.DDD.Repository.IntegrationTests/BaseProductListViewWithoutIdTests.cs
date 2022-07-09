namespace Dapper.DDD.Repository.IntegrationTests;
public abstract class BaseProductListViewWithoutIdTests
{
	private readonly IViewRepository<ProductListView> _repository;

	protected BaseProductListViewWithoutIdTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<IViewRepository<ProductListView>>()!;
	}

	[Fact]
	public async Task GetAll_HaveRows_Valid()
	{
		// Act
		var all = await _repository.GetAllAsync();

		// Assert
		Assert.True(all.Count() >= 2);
	}
}
