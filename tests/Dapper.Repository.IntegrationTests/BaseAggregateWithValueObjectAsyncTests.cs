using Dapper.Repository.IntegrationTests.Repositories;

namespace Dapper.Repository.IntegrationTests;
public abstract class BaseAggregateWithValueObjectAsyncTests<TDbException>
where TDbException : Exception
{
	private readonly ICustomerRepository _repository;

	protected BaseAggregateWithValueObjectAsyncTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<ICustomerRepository>()!;
	}

	#region Delete

	[Fact]
	public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Act
		var deleted = await _repository.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.Null(deleted);
	}

	[Theory, AutoDomainData]
	public async Task Delete_UsePrimaryKey_Valid(Customer aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var deleted = await _repository.DeleteAsync(insertedAggregate.Id);

		// Assert
		Assert.Equal(insertedAggregate, deleted);
		Assert.NotSame(insertedAggregate, deleted);
	}
	#endregion

	#region Get

	[Theory, AutoDomainData]
	public async Task Get_UsePrimaryKey_Valid(Customer aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var gotten = await _repository.GetAsync(insertedAggregate.Id);

		// Assert
		Assert.Equal(insertedAggregate, gotten);
		Assert.NotSame(insertedAggregate, gotten);

		_ = await _repository.DeleteAsync(insertedAggregate.Id);
	}

	[Fact]
	public async Task Get_UseMissingPrimaryKey_ReturnsNull()
	{
		// Act
		var gotten = await _repository.GetAsync(Guid.NewGuid());

		// Assert
		Assert.Null(gotten);
	}
	#endregion

	#region GetAll
	[Theory, AutoDomainData]
	public async Task GetAll_NoInput_Valid(Customer aggregate)
	{
		// Arrange
		_ = await _repository.InsertAsync(aggregate);

		// Act
		var fetchedEntities = await _repository.GetAllAsync();

		// Assert
		Assert.True(fetchedEntities.Count() > 0);
	}
	#endregion

	#region Insert
	[Fact]
	public async Task Insert_InputIsNull_Throws()
	{
		// Act && Assert
		_ = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.InsertAsync(null!));
	}

	[Theory, AutoDomainData]
	public async Task Insert_HasAllValues_IsInserted(Customer aggregate)
	{
		// Act
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		try
		{
			// Assert
			Assert.Equal(aggregate, insertedAggregate);
			Assert.NotSame(aggregate, insertedAggregate);
		}
		finally
		{
			_ = await _repository.DeleteAsync(insertedAggregate.Id);
		}
	}

	[Theory, AutoDomainData]
	public async Task Insert_NonNullPropertyMissing_Throws(Customer aggregate)
	{
		// Arrange
		aggregate = aggregate with { InvoiceAddress = aggregate.InvoiceAddress with { Street = null! } };

		// Act && Assert
		_ = await Assert.ThrowsAsync<TDbException>(async () => await _repository.InsertAsync(aggregate));
	}
	#endregion

	#region Update
	[Fact]
	public async Task Update_InputIsNull_Throws()
	{
		// Act && Assert
		_ = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.UpdateAsync(null!));
	}

	[Theory, AutoDomainData]
	public async Task Update_UseAggregate_Valid(Customer aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		var update = insertedAggregate with { Name = "Some new name" };

		// Act
		var updatedAggregate = await _repository.UpdateAsync(update);

		// Assert
		Assert.Equal("Some new name", updatedAggregate?.Name);

		_ = await _repository.DeleteAsync(insertedAggregate.Id);
	}

	[Fact]
	public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Arrange
		var aggregate = new Customer
		{
			Id = Guid.NewGuid(),
			Name = "Hello world",
			InvoiceAddress = new Address
			{
				Street = "Road",
				Zipcode = 1200
			}
		};

		// Act 
		var updated = await _repository.UpdateAsync(aggregate);

		// Assert
		Assert.Null(updated);
	}
	#endregion
}
