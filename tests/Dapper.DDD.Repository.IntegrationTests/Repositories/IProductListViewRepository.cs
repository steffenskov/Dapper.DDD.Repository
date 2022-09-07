namespace Dapper.DDD.Repository.IntegrationTests.Repositories;

public interface IProductListViewRepository : IViewRepository<ProductListView, int>
{
	Task<ProductListView?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

	Task<int?> GetProductIdByNameAsync(string name, CancellationToken cancellationToken = default);
}