using Dapper.Repository.Configuration;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace Dapper.Repository.IntegrationTests.Repositories;
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
