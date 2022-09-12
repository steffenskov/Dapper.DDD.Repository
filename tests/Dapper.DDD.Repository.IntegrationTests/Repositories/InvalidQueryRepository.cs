using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.IntegrationTests.Repositories;

public interface IInvalidQueryRepository
{
	Task ExecuteAsync();
	Task QuerySingleAsync();
	Task QuerySingleOrDefaultAsync();
	Task QueryAsync();
	Task ScalarSingleAsync();
	Task ScalarSingleOrDefaultAsync();
	Task ScalarMultipleAsync();
}

public class InvalidQueryRepository : ViewRepository<string>, IInvalidQueryRepository
{
	public InvalidQueryRepository(IOptions<ViewAggregateConfiguration<string>> options,
		IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task ExecuteAsync()
	{
		await ExecuteAsync("Invalid query");
	}

	public async Task QuerySingleAsync()
	{
		await QuerySingleAsync("Invalid query");
	}

	public async Task QuerySingleOrDefaultAsync()
	{
		await QuerySingleOrDefaultAsync("Invalid query");
	}

	public async Task QueryAsync()
	{
		await QueryAsync("Invalid query");
	}

	public async Task ScalarSingleAsync()
	{
		await ScalarSingleAsync<int>("Invalid query");
	}

	public async Task ScalarSingleOrDefaultAsync()
	{
		await ScalarSingleOrDefaultAsync<int>("Invalid query");
	}

	public async Task ScalarMultipleAsync()
	{
		await ScalarMultipleAsync<int>("Invalid query");
	}
}