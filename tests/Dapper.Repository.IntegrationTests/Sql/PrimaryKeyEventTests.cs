using System;
using System.Linq;
using Dapper.Repository.Exceptions;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class PrimaryKeyEventTests
	{

		#region Delete
		[Theory, AutoDomainData]
		public void Delete_PreInsertHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PreDelete += (inputEntity, cancelArgs) =>
			{
				// Assert
				Assert.Equal(entity, inputEntity);
			};

			// Act
			repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Delete_PreInsertThrows_IsDeleted(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PreDelete += (inputEntity, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var deletedEntity = repository.Delete(insertedEntity);

			// Assert
			Assert.True(deletedEntity?.CategoryId > 0);
			var gotten = repository.Get(deletedEntity!);
			Assert.Null(gotten);
			Assert.Equal(deletedEntity, insertedEntity);
			Assert.NotSame(deletedEntity, insertedEntity); // Ensure we're not just handed the inputEntity back
		}

		[Theory, AutoDomainData]
		public void Delete_PreInsertCancels_IsCanceled(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			var shouldCancel = true;

			repository.PreDelete += (inputEntity, cancelArgs) =>
			{
				cancelArgs.Cancel = shouldCancel;
			};

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Delete(insertedEntity));

			// Assert
			Assert.NotNull(repository.Get(insertedEntity));

			// Cleanup
			shouldCancel = false;
			repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Delete_PostDeleteHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			CategoryEntity? deletedEntity = null;
			repository.PostDelete += (tmpEntity) =>
			{
				deletedEntity = tmpEntity;
			};

			// Act
			var result = repository.Delete(insertedEntity);

			// Assert
			Assert.Equal(result, deletedEntity);
			Assert.Same(result, deletedEntity);
		}

		[Theory, AutoDomainData]
		public void Delete_PostDeleteThrows_IsDeleted(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PostDelete += (tmpEntity) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var result = repository.Delete(insertedEntity);

			// Assert
			Assert.True(result?.CategoryId > 0);
			var gotten = repository.Get(result!);
			Assert.Null(gotten);
		}
		#endregion

		#region Insert
		[Theory, AutoDomainData]
		public void Insert_PreInsertHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertEntity, cancelArgs) =>
			{
				// Assert
				Assert.Equal(entity, preInsertEntity);
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Insert_PreInsertThrows_IsInserted(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertEntity, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			// Assert
			try
			{
				Assert.True(insertedEntity.CategoryId > 0);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_PreInsertCancels_IsCancelled(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertEntity, cancelArgs) =>
			{
				cancelArgs.Cancel = true;
			};

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Insert(entity));

			var allNames = repository.GetAll().Select(category => category.Name);
			Assert.DoesNotContain(entity.Name, allNames);
		}

		[Theory, AutoDomainData]
		public void Insert_PostInsertHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			CategoryEntity? postInsertEntity = null;
			repository.PostInsert += (tmpEntity) =>
			{
				postInsertEntity = tmpEntity;
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			// Assert
			try
			{
				Assert.Equal(insertedEntity, postInsertEntity);
				Assert.Same(insertedEntity, postInsertEntity);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_PostInsertThrows_IsInserted(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PostInsert += (tmpEntity) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			// Assert
			try
			{
				Assert.True(insertedEntity.CategoryId > 0);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}
		#endregion

		#region Update
		[Theory, AutoDomainData]
		public void Update_PreUpdateHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			var insertedEntity = repository.Insert(entity);

			var entityToUpdate = insertedEntity with { Description = "Hello world" };

			repository.PreUpdate += (preUpdateEntity, cancelArgs) =>
			{
				// Assert
				Assert.Equal(entityToUpdate, preUpdateEntity);
			};

			// Act
			repository.Update(entityToUpdate);

			repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Update_PreUpdateThrows_IsUpdated(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PreUpdate += (preUpdateEntity, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var updatedEntity = repository.Update(insertedEntity with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal("Hello world", updatedEntity?.Description);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Update_PreUpdateCancels_IsCancelled(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreUpdate += (preUpdateEntity, cancelArgs) =>
			{
				cancelArgs.Cancel = true;
			};

			var insertedEntity = repository.Insert(entity);

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Update(insertedEntity with { Description = "Hello world" }));
			var gottenEntity = repository.Get(insertedEntity);
			Assert.Equal(entity.Description, gottenEntity?.Description);

			repository.Delete(insertedEntity);
		}

		[Theory, AutoDomainData]
		public void Update_PostUpdateHasEvent_IsInvoked(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			CategoryEntity? postUpdateEntity = null;
			repository.PostUpdate += (tmpEntity) =>
			{
				postUpdateEntity = tmpEntity;
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			var updatedEntity = repository.Update(insertedEntity with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal(updatedEntity, postUpdateEntity);
				Assert.Same(updatedEntity, postUpdateEntity);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Update_PostUpdatedThrows_IsUpdated(CategoryEntity entity)
		{
			// Arrange
			var repository = new CategoryRepository();

			var insertedEntity = repository.Insert(entity);

			repository.PostUpdate += (tmpEntity) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var updatedEntity = repository.Update(insertedEntity with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal("Hello world", updatedEntity?.Description);
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}
		#endregion
	}
}
