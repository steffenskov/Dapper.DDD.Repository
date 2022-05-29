namespace Dapper.Repository.IntegrationTests;
public abstract class BaseViewTests
{
	private readonly ITableRepository<ProductListView, int> _repository;

	protected BaseViewTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<ITableRepository<ProductListView, int>>()!;
	}

	[Fact]
	public async Task GetAll_HaveRows_Valid()
	{
		// Act
		var all = await _repository.GetAllAsync();

		Assert.True(all.Count() >= 2);
	}
}
