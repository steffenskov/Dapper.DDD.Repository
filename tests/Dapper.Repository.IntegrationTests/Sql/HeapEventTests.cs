using System;
using Dapper.Repository.Exceptions;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class HeapEventTests
	{

		#region Delete
		[Theory, AutoDomainData]
		public void Delete_PreInsertHasEvent_IsInvoked(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();
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
		public void Delete_PreInsertThrows_IsDeleted(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PreDelete += (inputEntity, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var deletedEntity = repository.Delete(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity, deletedEntity);
			var gotten = repository.Get(deletedEntity!);
			Assert.Null(gotten);
			Assert.NotSame(insertedEntity, deletedEntity); // Ensure we're not just handed the inputEntity back
		}

		[Theory, AutoDomainData]
		public void Delete_PreInsertCancels_IsCanceled(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();
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
		public void Delete_PostDeleteHasEvent_IsInvoked(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();
			var insertedEntity = repository.Insert(entity);

			UserHeapEntity? deletedEntity = null;
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
		public void Delete_PostDeleteThrows_IsDeleted(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();
			var insertedEntity = repository.Insert(entity);

			repository.PostDelete += (tmpEntity) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var result = repository.Delete(insertedEntity);

			// Assert
			Assert.Equal(insertedEntity, result);
			var gotten = repository.Get(result!);
			Assert.Null(gotten);
		}
		#endregion

		#region Insert
		[Theory, AutoDomainData]
		public void Insert_PreInsertHasEvent_IsInvoked(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();

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
		public void Insert_PreInsertThrows_IsInserted(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();

			repository.PreInsert += (preInsertEntity, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			// Assert
			try
			{
				Assert.Equal(insertedEntity, repository.Get(entity));
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_PreInsertCancels_IsCancelled(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();

			repository.PreInsert += (preInsertEntity, cancelArgs) =>
			{
				cancelArgs.Cancel = true;
			};

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Insert(entity));

			var gotten = repository.Get(entity);
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public void Insert_PostInsertHasEvent_IsInvoked(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();

			UserHeapEntity? postInsertEntity = null;
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
		public void Insert_PostInsertThrows_IsInserted(UserHeapEntity entity)
		{
			// Arrange
			var repository = new UserHeapRepository();

			repository.PostInsert += (tmpEntity) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedEntity = repository.Insert(entity);

			// Assert
			try
			{
				Assert.Equal(insertedEntity, repository.Get(entity));
			}
			finally
			{
				repository.Delete(insertedEntity);
			}
		}
		#endregion
	}
}
