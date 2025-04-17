namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseProductListViewWithoutIdTests : BaseTests
{
	private readonly IViewRepository<ProductListView> _repository;
	
	protected BaseProductListViewWithoutIdTests(IContainerFixture fixture) : base(fixture)
	{
		_repository = fixture.Provider.GetRequiredService<IViewRepository<ProductListView>>();
	}
	
	[Fact]
	public async Task GetAll_HaveRows_Valid()
	{
		// Act
		var all = await _repository.GetAllAsync(TestContext.Current.CancellationToken);
		
		// Assert
		Assert.True(all.Count() >= 2);
	}
}