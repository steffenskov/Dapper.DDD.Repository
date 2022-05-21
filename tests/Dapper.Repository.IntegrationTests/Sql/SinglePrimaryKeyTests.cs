using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class SinglePrimaryKeyTests
	{
		private readonly CategoryRepository _repository;

		public SinglePrimaryKeyTests()
		{
			_repository = new CategoryRepository();
		}

		#region Delete
		[Fact]
		public void Delete_InputIsNull_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => _repository.Delete(null!));
		}

		[Theory, AutoDomainData]
		public void Delete_PrimaryKeyNotEntered_Throws(CategoryEntity entity)
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Delete(entity));
		}

		[Fact]
		public void Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act 
			var deleted = _repository.Delete(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public void Delete_UsePrimaryKey_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var deleted = _repository.Delete(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted?.CategoryId);
			Assert.Equal(entity.Description, deleted?.Description);
			Assert.Equal(entity.Name, deleted?.Name);
			Assert.Equal(entity.Picture, deleted?.Picture);
		}

		[Theory, AutoDomainData]
		public void Delete_UseEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var deleted = _repository.Delete(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.CategoryId, deleted?.CategoryId);
			Assert.Equal(entity.Description, deleted?.Description);
			Assert.Equal(entity.Name, deleted?.Name);
			Assert.Equal(entity.Picture, deleted?.Picture);
		}
		#endregion

		#region Get
		[Fact]
		public void Get_InputIsNull_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => _repository.Get(null!));
		}

		[Theory, AutoDomainData]
		public void Get_UsePrimaryKey_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var fetchedEntity = _repository.Get(new CategoryPrimaryKeyEntity { CategoryId = insertedEntity.CategoryId });

			// Assert
			Assert.Equal(insertedEntity.Name, fetchedEntity?.Name);
			Assert.Equal(insertedEntity.Description, fetchedEntity?.Description);
			Assert.Equal(insertedEntity.Picture, fetchedEntity?.Picture);

			_repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Get_UseFullEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var fetchedEntity = _repository.Get(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity.Description, fetchedEntity?.Description);
			Assert.Equal(insertedEntity.Name, fetchedEntity?.Name);
			Assert.Equal(insertedEntity.Picture, fetchedEntity?.Picture);

			_repository.Delete(insertedEntity);
		}

		[Fact]
		public void Get_PrimaryKeyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Get(new CategoryPrimaryKeyEntity { }));
		}

		[Fact]
		public void Get_UseMissingPrimaryKey_ReturnsNull()
		{
			// Act
			var gotten = _repository.Get(new CategoryPrimaryKeyEntity { CategoryId = int.MaxValue });

			// Assert
			Assert.Null(gotten);
		}
		#endregion

		#region GetAll
		[Fact]
		public void GetAll_NoInput_Valid()
		{
			// Act
			var fetchedEntities = _repository.GetAll();

			// Assert
			Assert.True(fetchedEntities.Count() > 0);
		}
		#endregion

		#region Insert
		[Fact]
		public void Insert_InputIsNull_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => _repository.Insert(null!));
		}

		[Fact]
		public void Insert_HasIdentityKeyWithValue_Throws()
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
			Assert.Throws<ArgumentException>(() => _repository.Insert(entity));
		}

		[Theory, AutoDomainData]
		public void Insert_HasIdentityKeyWithoutValue_IsInserted(CategoryEntity entity)
		{
			// Act
			var insertedEntity = _repository.Insert(entity);
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
				_repository.Delete(insertedEntity);
			}
		}

		[Fact]
		public void Insert_NonNullPropertyMissing_Throws()
		{
			// Arrange
			var entity = new CategoryEntity
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = null!,
				Picture = null
			};

			// Act && Assert
			Assert.Throws<SqlException>(() => _repository.Insert(entity));
		}
		#endregion

		#region Update
		[Fact]
		public void Update_InputIsNull_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => _repository.Update(null!));
		}

		[Theory, AutoDomainData]
		public void Update_UseEntity_Valid(CategoryEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			var update = insertedEntity with { Description = "Something else" };

			// Act
			var updatedEntity = _repository.Update(update);

			// Assert
			Assert.Equal("Something else", updatedEntity?.Description);

			_repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Update_PrimaryKeyNotEntered_Throws(CategoryEntity entity)
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Update(entity));
		}

		[Fact]
		public void Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Arrange
			var entity = new CategoryEntity
			{
				CategoryId = int.MaxValue,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act
			var updated = _repository.Update(entity);

			// Assert
			Assert.Null(updated);
		}
		#endregion
	}
}
