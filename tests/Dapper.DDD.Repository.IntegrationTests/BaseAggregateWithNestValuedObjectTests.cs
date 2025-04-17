namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseAggregateWithNestedValueObjectTests : BaseTests
{
	private readonly ITableRepository<CustomerWithNestedAddresses, Guid> _repository;

	protected BaseAggregateWithNestedValueObjectTests(IContainerFixture fixture) : base(fixture)
	{
		_repository = fixture.Provider.GetRequiredService<ITableRepository<CustomerWithNestedAddresses, Guid>>();
	}

	[Theory]
	[AutoDomainData]
	public async Task Delete_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Act
		var deleted = await _repository.DeleteAsync(aggregate.Id, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotSame(aggregate, deleted);
		Assert.Equal(aggregate, deleted);
	}

	[Theory]
	[AutoDomainData]
	public async Task Insert_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange && Act
		var inserted = await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotSame(aggregate, inserted);
		Assert.Equal(aggregate, inserted);
	}

	[Theory]
	[AutoDomainData]
	public async Task Get_UsePrimaryKey_Valid(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Act
		var fetched = await _repository.GetAsync(aggregate.Id, TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(aggregate, fetched);
	}

	[Theory]
	[AutoDomainData]
	public async Task GetAll_Valid_ContainsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Act
		var fetched = (await _repository.GetAllAsync(TestContext.Current.CancellationToken)).Single(x => x.Id == aggregate.Id);

		// Assert
		Assert.Equal(aggregate, fetched);
	}

	[Theory]
	[AutoDomainData]
	public async Task Update_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Act
		var toUpdate = aggregate with
		{
			Addresses = new Addresses(new Address("Other name", new Zipcode(Random.Shared.Next(int.MaxValue))),
				new Address("Other name", new Zipcode(42)))
		};
		var updated = await _repository.UpdateAsync(toUpdate, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotSame(toUpdate, updated);
		Assert.Equal(toUpdate, updated);
	}

	[Theory]
	[AutoDomainData]
	public async Task Upsert_DoesNotExist_Inserted(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		var exists = await _repository.GetAsync(aggregate.Id, TestContext.Current.CancellationToken);
		Assert.Null(exists);

		// Act
		var inserted = await _repository.UpsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotSame(aggregate, inserted);
		Assert.Equal(aggregate, inserted);
	}

	[Theory]
	[AutoDomainData]
	public async Task Upsert_Exists_Updated(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		var inserted = await _repository.InsertAsync(aggregate, TestContext.Current.CancellationToken);

		// Act
		var toUpdate = aggregate with
		{
			Addresses = new Addresses(new Address("Other name", new Zipcode(Random.Shared.Next(int.MaxValue))),
				new Address("Other name", new Zipcode(1337)))
		};
		var updated = await _repository.UpsertAsync(toUpdate, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotSame(toUpdate, updated);
		Assert.Equal(toUpdate, updated);
	}
}