using Dapper.Repository.Configuration;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace Dapper.Repository.IntegrationTests.Repositories;

internal class CustomerRepository : TableRepository<Customer, Guid>, ICustomerRepository
{
	public CustomerRepository(IOptions<TableAggregateConfiguration<Customer>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task<Customer?> GetByNameAsync(string name)
	{
		return await QuerySingleOrDefaultAsync($"SELECT * FROM {EntityName} WHERE Name = @name", new { name });
	}
}
