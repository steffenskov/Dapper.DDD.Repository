namespace Dapper.Repository.IntegrationTests.Repositories;
public interface IProductListViewRepository : IViewRepository<ProductListView, int>
{
	Task<ProductListView?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
