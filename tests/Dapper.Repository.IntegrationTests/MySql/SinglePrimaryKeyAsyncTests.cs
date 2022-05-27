using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using MySql.Data.MySqlClient;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
{
	public class SinglePrimaryKeyAsyncTests
	{
		private readonly CategoryRepository _repository;

		public SinglePrimaryKeyAsyncTests()
		{
			_repository = new CategoryRepository();
		}

		#region Delete
		[Fact]
		public async Task Delete_InputIsNull_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.DeleteAsync(null!));
		}

		[Theory, AutoDomainData]
		public async Task Delete_PrimaryKeyNotEntered_Throws(Category aggregate)
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(aggregate));
		}

		[Fact]
		public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = await _repository.DeleteAsync(new CategoryPrimaryKeyAggregate { CategoryId = int.MaxValue });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UsePrimaryKey_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = await _repository.InsertAsync(aggregate);

			// Act
			var deleted = await _repository.DeleteAsync(new CategoryPrimaryKeyAggregate { CategoryId = insertedAggregate.CategoryId });

			// Assert
			Assert.Equal(insertedAggregate.CategoryId, deleted?.CategoryId);
			Assert.Equal(aggregate.Description, deleted?.Description);
			Assert.Equal(aggregate.Name, deleted?.Name);
			Assert.Equal(aggregate.Picture, deleted?.Picture);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UseAggregate_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = await _repository.InsertAsync(aggregate);

			// Act
			var deleted = await _repository.DeleteAsync(insertedAggregate);

			// Assert
			Assert.Equal(insertedAggregate.CategoryId, deleted?.CategoryId);
			Assert.Equal(aggregate.Description, deleted?.Description);
			Assert.Equal(aggregate.Name, deleted?.Name);
			Assert.Equal(aggregate.Picture, deleted?.Picture);
		}
		#endregion

		#region Get
		[Fact]
		public async Task Get_InputIsNull_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.GetAsync(null!));
		}

		[Theory, AutoDomainData]
		public async Task Get_UsePrimaryKey_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = await _repository.InsertAsync(aggregate);

			// Act
			var fetchedAggregate = await _repository.GetAsync(new CategoryPrimaryKeyAggregate { CategoryId = insertedAggregate.CategoryId });

			// Assert
			Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
			Assert.Equal(insertedAggregate.Name, fetchedAggregate?.Name);
			Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

			await _repository.DeleteAsync(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public async Task Get_UseFullAggregate_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = await _repository.InsertAsync(aggregate);

			// Act
			var fetchedAggregate = await _repository.GetAsync(insertedAggregate);

			// Assert
			Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
			Assert.Equal(insertedAggregate.Name, fetchedAggregate?.Name);
			Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

			await _repository.DeleteAsync(insertedAggregate);
		}

		[Fact]
		public async Task Get_PrimaryKeyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.GetAsync(new CategoryPrimaryKeyAggregate { }));
		}

		[Fact]
		public async Task Get_UseMissingPrimaryKey_ReturnsNull()
		{
			// Act
			var gotten = await _repository.GetAsync(new CategoryPrimaryKeyAggregate { CategoryId = int.MaxValue });

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
		public async Task Insert_HasIdaggregateKeyWithValue_Throws()
		{
			// Arrange
			var aggregate = new Category
			{
				CategoryId = 42,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};

			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.InsertAsync(aggregate));
		}

		[Theory, AutoDomainData]
		public async Task Insert_HasIdaggregateKeyWithoutValue_IsInserted(Category aggregate)
		{
			// Act
			var insertedAggregate = await _repository.InsertAsync(aggregate);
			try
			{
				// Assert
				Assert.NotEqual(default, insertedAggregate.CategoryId);
				Assert.Equal(aggregate.Description, insertedAggregate.Description);
				Assert.Equal(aggregate.Name, insertedAggregate.Name);
				Assert.Equal(aggregate.Picture, insertedAggregate.Picture);
			}
			finally
			{
				await _repository.DeleteAsync(insertedAggregate);
			}
		}

		[Fact]
		public async Task Insert_NonNullPropertyMissing_Throws()
		{
			// Arrange
			var aggregate = new Category
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = null!,
				Picture = null
			};

			// Act && Assert
			await Assert.ThrowsAsync<MySqlException>(async () => await _repository.InsertAsync(aggregate));
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

			await _repository.DeleteAsync(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public async Task Update_PrimaryKeyNotEntered_Throws(Category aggregate)
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.UpdateAsync(aggregate));
		}

		[Fact]
		public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Arrange
			var aggregate = new Category
			{
				CategoryId = int.MaxValue,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act 
			var updated = await _repository.UpdateAsync(aggregate);

			// Assert
			Assert.Null(updated);
		}
		#endregion
	}
}
