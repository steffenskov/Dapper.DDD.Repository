using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.IntegrationTests.Repositories;
public class ProductListViewRepository : ViewRepository<ProductListView, int>, IProductListViewRepository
{
	public ProductListViewRepository(IOptions<ViewAggregateConfiguration<ProductListView>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task<ProductListView?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await QuerySingleOrDefaultAsync($"SELECT * FROM {ViewName} WHERE ProductName = @name", new { name }, cancellationToken: cancellationToken);
	}
}
