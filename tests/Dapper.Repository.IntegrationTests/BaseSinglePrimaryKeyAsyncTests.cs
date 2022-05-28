namespace Dapper.Repository.IntegrationTests;
public abstract class BaseSinglePrimaryKeyAsyncTests<TDbException>
where TDbException : Exception
{
	private readonly ITableRepository<Category, int> _repository;

	protected BaseSinglePrimaryKeyAsyncTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<ITableRepository<Category, int>>()!;
	}

	#region Delete

	[Fact]
	public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Act
		var deleted = await _repository.DeleteAsync(int.MaxValue);

		// Assert
		Assert.Null(deleted);
	}

	[Theory, AutoDomainData]
	public async Task Delete_UsePrimaryKey_Valid(Category aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var deleted = await _repository.DeleteAsync(insertedAggregate.CategoryID);

		// Assert
		Assert.Equal(insertedAggregate.CategoryID, deleted?.CategoryID);
		Assert.Equal(aggregate.Description, deleted?.Description);
		Assert.Equal(aggregate.CategoryName, deleted?.CategoryName);
		Assert.Equal(aggregate.Picture, deleted?.Picture);
	}

	[Theory, AutoDomainData]
	public async Task Delete_UseAggregate_Valid(Category aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var deleted = await _repository.DeleteAsync(insertedAggregate.CategoryID);

		// Assert
		Assert.Equal(insertedAggregate.CategoryID, deleted?.CategoryID);
		Assert.Equal(aggregate.Description, deleted?.Description);
		Assert.Equal(aggregate.CategoryName, deleted?.CategoryName);
		Assert.Equal(aggregate.Picture, deleted?.Picture);
	}
	#endregion

	#region Get

	[Theory, AutoDomainData]
	public async Task Get_UsePrimaryKey_Valid(Category aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var fetchedAggregate = await _repository.GetAsync(insertedAggregate.CategoryID);

		// Assert
		Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
		Assert.Equal(insertedAggregate.CategoryName, fetchedAggregate?.CategoryName);
		Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

		await _repository.DeleteAsync(insertedAggregate.CategoryID);
	}

	[Theory, AutoDomainData]
	public async Task Get_UseFullAggregate_Valid(Category aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Act
		var fetchedAggregate = await _repository.GetAsync(insertedAggregate.CategoryID);

		// Assert
		Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
		Assert.Equal(insertedAggregate.CategoryName, fetchedAggregate?.CategoryName);
		Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

		await _repository.DeleteAsync(insertedAggregate.CategoryID);
	}

	[Fact]
	public async Task Get_UseMissingPrimaryKey_ReturnsNull()
	{
		// Act
		var gotten = await _repository.GetAsync(int.MaxValue);

		// Assert
		Assert.Null(gotten);
	}
	#endregion

	#region GetAll
	[Fact]
	public async Task GetAll_NoInput_Valid()
	{
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
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.InsertAsync(null!));
	}

	[Fact]
	public async Task Insert_HasidentityKeyWithValue_Throws()
	{
		// Arrange
		var aggregate = new Category
		{
			CategoryID = 42,
			Description = "Lorem ipsum, dolor sit amit",
			CategoryName = "Lorem ipsum",
			Picture = null
		};

		// Act && Assert
		await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.InsertAsync(aggregate));
	}

	[Theory, AutoDomainData]
	public async Task Insert_HasidentityKeyWithoutValue_IsInserted(Category aggregate)
	{
		// Act
		var insertedAggregate = await _repository.InsertAsync(aggregate);
		try
		{
			// Assert
			Assert.NotEqual(default, insertedAggregate.CategoryID);
			Assert.Equal(aggregate.Description, insertedAggregate.Description);
			Assert.Equal(aggregate.CategoryName, insertedAggregate.CategoryName);
			Assert.Equal(aggregate.Picture, insertedAggregate.Picture);
		}
		finally
		{
			await _repository.DeleteAsync(insertedAggregate.CategoryID);
		}
	}

	[Fact]
	public async Task Insert_NonNullPropertyMissing_Throws()
	{
		// Arrange
		var aggregate = new Category
		{
			Description = "Lorem ipsum, dolor sit amit",
			CategoryName = null!,
			Picture = null
		};

		// Act && Assert
		await Assert.ThrowsAsync<TDbException>(async () => await _repository.InsertAsync(aggregate));
	}
	#endregion

	#region Update
	[Fact]
	public async Task Update_InputIsNull_Throws()
	{
		// Act && Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.UpdateAsync(null!));
	}

	[Theory, AutoDomainData]
	public async Task Update_UseAggregate_Valid(Category aggregate)
	{
		// Arrange
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		var update = insertedAggregate with { Description = "Something else" };

		// Act
		var updatedAggregate = await _repository.UpdateAsync(update);

		// Assert
		Assert.Equal("Something else", updatedAggregate?.Description);

		await _repository.DeleteAsync(insertedAggregate.CategoryID);
	}

	[Fact]
	public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
	{
		// Arrange
		var aggregate = new Category
		{
			CategoryID = int.MaxValue,
			Description = "Lorem ipsum, dolor sit amit",
			CategoryName = "Hello world"
		};

		// Act 
		var updated = await _repository.UpdateAsync(aggregate);

		// Assert
		Assert.Null(updated);
	}
	#endregion
}
