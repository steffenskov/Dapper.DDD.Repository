using Dapper.DDD.Repository.Exceptions;
using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseInvalidQueryTests
{
	private readonly IInvalidQueryRepository _repository;

	protected BaseInvalidQueryTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<IInvalidQueryRepository>()!;
	}

	[Fact]
	public async Task ExecuteAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.ExecuteAsync());
	}

	[Fact]
	public async Task QueryAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.QueryAsync());
	}

	[Fact]
	public async Task QuerySingleAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.QuerySingleAsync());
	}

	[Fact]
	public async Task QuerySingleOrDefaultAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.QuerySingleOrDefaultAsync());
	}

	[Fact]
	public async Task ScalarMultipleAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.ScalarMultipleAsync());
	}

	[Fact]
	public async Task ScalarSingleAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.ScalarSingleAsync());
	}

	[Fact]
	public async Task ScalarSingleOrDefaultAsync_InvalidQuery_ThrowsDapperException()
	{
		// Act && Assert
		var ex = await Assert.ThrowsAsync<DapperRepositoryQueryException>(async () =>
			await _repository.ScalarSingleOrDefaultAsync());
	}
}