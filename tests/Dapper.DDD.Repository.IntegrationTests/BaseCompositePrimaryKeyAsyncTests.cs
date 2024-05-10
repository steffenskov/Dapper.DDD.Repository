namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseCompositePrimaryKeyAsyncTests : BaseTests
{
	private readonly ITableRepository<CompositeUser, CompositeUserId> _repository;
	
	protected BaseCompositePrimaryKeyAsyncTests(IContainerFixture fixture) : base(fixture)
	{
		_repository = fixture.Provider.GetRequiredService<ITableRepository<CompositeUser, CompositeUserId>>();
	}
	
	
	[Fact]
	public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Act
		var deleted = await _repository.DeleteAsync(new CompositeUserId("async My name", "Secret"));
		
		// Assert
		Assert.Null(deleted);
	}
	
	[Theory]
	[AutoDomainData]
	public async Task Delete_UsePrimaryKey_Valid(CompositeUser aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		
		// Act
		var deleted =
			await _repository.DeleteAsync(new CompositeUserId(aggregate.Id.Username, aggregate.Id.Password));
		
		// Assert
		Assert.Equal(aggregate.Id.Username, deleted?.Id.Username);
		Assert.Equal(aggregate.Id.Password, deleted?.Id.Password);
		Assert.Equal(insertedAggregate.DateCreated, deleted?.DateCreated);
	}
	
	[Fact]
	public async Task Get_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Act
		var gotten = await _repository.GetAsync(new CompositeUserId("async My name", "Secret"));
		
		// Assert
		Assert.Null(gotten);
	}
	
	[Theory]
	[AutoDomainData]
	public async Task Get_UsePrimaryKey_Valid(CompositeUser aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		
		// Act
		var gotten = await _repository.GetAsync(aggregate.Id);
		
		// Assert
		Assert.Equal(insertedAggregate, gotten);
		Assert.NotSame(insertedAggregate, gotten);
		Assert.Equal(aggregate.Id.Username, gotten?.Id.Username);
		Assert.Equal(aggregate.Id.Password, gotten?.Id.Password);
		
		await _repository.DeleteAsync(insertedAggregate.Id);
	}
	
	[Theory]
	[AutoDomainData]
	public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull(CompositeUser aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		
		// Act
		var updated = await _repository.UpdateAsync(insertedAggregate with
		{
			Id = new CompositeUserId("Doesnt exist", "Secret")
		});
		
		// Assert
		Assert.Null(updated);
		
		await _repository.DeleteAsync(insertedAggregate.Id);
	}
	
	[Theory]
	[AutoDomainData]
	public async Task Update_UsePrimaryKey_Valid(CompositeUser aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		
		// Act
		var updated = await _repository.UpdateAsync(insertedAggregate with { Age = 42 });
		
		// Assert
		Assert.Equal(aggregate.Id.Username, updated?.Id.Username);
		Assert.Equal(aggregate.Id.Password, updated?.Id.Password);
		Assert.NotEqual(42, insertedAggregate.Age);
		Assert.Equal(42, updated?.Age);
		Assert.Equal(insertedAggregate.DateCreated, updated?.DateCreated);
		
		await _repository.DeleteAsync(insertedAggregate.Id);
	}
}