namespace Dapper.DDD.Repository.IntegrationTests;
public abstract class BaseAggregateWithNestedValueObjectTests
{
	private ITableRepository<CustomerWithNestedAddresses, Guid> _repository;

	protected BaseAggregateWithNestedValueObjectTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<ITableRepository<CustomerWithNestedAddresses, Guid>>()!;
	}

	[Theory, AutoDomainData]
	public async Task Delete_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate);

		// Act
		var deleted = await _repository.DeleteAsync(aggregate.Id);

		// Assert
		Assert.NotSame(aggregate, deleted);
		Assert.Equal(aggregate, deleted);
	}

	[Theory, AutoDomainData]
	public async Task Insert_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange && Act
		var inserted = await _repository.InsertAsync(aggregate);

		// Assert
		Assert.NotSame(aggregate, inserted);
		Assert.Equal(aggregate, inserted);
	}

	[Theory, AutoDomainData]
	public async Task Get_UsePrimaryKey_Valid(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate);

		// Act
		var fetched = await _repository.GetAsync(aggregate.Id);

		// Assert
		Assert.Equal(aggregate, fetched);
	}

	[Theory, AutoDomainData]
	public async Task GetAll_Valid_ContainsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate);

		// Act
		var fetched = (await _repository.GetAllAsync()).Single(x => x.Id == aggregate.Id);

		// Assert
		Assert.Equal(aggregate, fetched);
	}

	[Theory, AutoDomainData]
	public async Task Update_Valid_ReturnsNestedValues(CustomerWithNestedAddresses aggregate)
	{
		// Arrange
		await _repository.InsertAsync(aggregate);

		// Act
		var toUpdate = aggregate with { Addresses = new Addresses(new Address("Other name", new Zipcode(Random.Shared.Next(int.MaxValue))), new Address("Other name", Zipcode.New())) };
		var updated = await _repository.UpdateAsync(toUpdate);

		// Assert
		Assert.NotSame(toUpdate, updated);
		Assert.Equal(toUpdate, updated);
	}
}