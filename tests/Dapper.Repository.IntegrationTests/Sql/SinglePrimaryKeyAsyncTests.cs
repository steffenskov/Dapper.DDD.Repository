using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
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
		public async Task Delete_PrimaryKeyNotEntered_Throws(CategoryEntity entity)
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(entity));
		}

		[Fact]
		public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = await _repository.DeleteAsync(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UsePrimaryKey_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = await _repository.InsertAsync(entity);

			// Act
			var deleted = await _repository.DeleteAsync(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted?.CategoryId);
			Assert.Equal(entity.Description, deleted?.Description);
			Assert.Equal(entity.Name, deleted?.Name);
			Assert.Equal(entity.Picture, deleted?.Picture);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UseEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = await _repository.InsertAsync(entity);

			// Act
			var deleted = await _repository.DeleteAsync(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted?.CategoryId);
			Assert.Equal(entity.Description, deleted?.Description);
			Assert.Equal(entity.Name, deleted?.Name);
			Assert.Equal(entity.Picture, deleted?.Picture);
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
		public async Task Get_UsePrimaryKey_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = await _repository.InsertAsync(entity);

			// Act
			var fetchedEntity = await _repository.GetAsync(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.Description, fetchedEntity?.Description);
			Assert.Equal(insertedEntity.Name, fetchedEntity?.Name);
			Assert.Equal(insertedEntity.Picture, fetchedEntity?.Picture);

			await _repository.DeleteAsync(insertedEntity);
		}

		[Theory, AutoDomainData]
		public async Task Get_UseFullEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = await _repository.InsertAsync(entity);

			// Act
			var fetchedEntity = await _repository.GetAsync(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.Description, fetchedEntity?.Description);
			Assert.Equal(insertedEntity.Name, fetchedEntity?.Name);
			Assert.Equal(insertedEntity.Picture, fetchedEntity?.Picture);

			await _repository.DeleteAsync(insertedEntity);
		}

		[Fact]
		public async Task Get_PrimaryKeyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.GetAsync(new CategoryPrimaryKeyEntity { }));
		}

		[Fact]
		public async Task Get_UseMissingPrimaryKey_ReturnsNull()
		{
			// Act
			var gotten = await _repository.GetAsync(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue });

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
		public async Task Insert_HasIdentityKeyWithValue_Throws()
		{
			// Arrange
			var entity = new CategoryEntity
			{
				CategoryId = 42,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Lorem ipsum",
				Picture = null
			};

			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.InsertAsync(entity));
		}

		[Theory, AutoDomainData]
		public async Task Insert_HasIdentityKeyWithoutValue_IsInserted(CategoryEntity entity)
		{
			// Act
			var insertedEntity = await _repository.InsertAsync(entity);
			try
			{
				// Assert
				Assert.NotEqual(default, insertedEntity.CategoryId);
				Assert.Equal(entity.Description, insertedEntity.Description);
				Assert.Equal(entity.Name, insertedEntity.Name);
				Assert.Equal(entity.Picture, insertedEntity.Picture);
			}
			finally
			{
				await _repository.DeleteAsync(insertedEntity);
			}
		}

		[Fact]
		public async Task Insert_NonNullPropertyMissing_Throws()
		{
			// Arrange
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = null!,
				Picture = null
			};

			// Act && Assert
			await Assert.ThrowsAsync<SqlException>(async () => await _repository.InsertAsync(entity));
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
		public async Task Update_UseEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = await _repository.InsertAsync(entity);

			var update = insertedEntity with { Description = "Something else" };

			// Act
			var updatedEntity = await _repository.UpdateAsync(update);

			// Assert
			Assert.Equal("Something else", updatedEntity?.Description);

			await _repository.DeleteAsync(insertedEntity);
		}

		[Theory, AutoDomainData]
		public async Task Update_PrimaryKeyNotEntered_Throws(CategoryEntity entity)
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.UpdateAsync(entity));
		}

		[Fact]
		public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Arrange
			var entity = new CategoryEntity
			{
				CategoryId = int.MaxValue,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act 
			var updated = await _repository.UpdateAsync(entity);

			// Assert
			Assert.Null(updated);
		}
		#endregion
	}
}
