using System;
using System.Linq;
using Dapper.Repository.Exceptions;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
{
	public class PrimaryKeyEventTests
	{

		#region Delete
		[Theory, AutoDomainData]
		public void Delete_PreInsertHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			repository.PreDelete += (inputAggregate, cancelArgs) =>
			{
				// Assert
				Assert.Equal(aggregate, inputAggregate);
			};

			// Act
			repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Delete_PreInsertThrows_IsDeleted(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			repository.PreDelete += (inputAggregate, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var deletedAggregate = repository.Delete(insertedAggregate);

			// Assert
			Assert.True(deletedAggregate?.CategoryId > 0);
			var gotten = repository.Get(deletedAggregate!);
			Assert.Null(gotten);
			Assert.Equal(deletedAggregate, insertedAggregate);
			Assert.NotSame(deletedAggregate, insertedAggregate); // Ensure we're not just handed the inputAggregate back
		}

		[Theory, AutoDomainData]
		public void Delete_PreInsertCancels_IsCanceled(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			var shouldCancel = true;

			repository.PreDelete += (inputAggregate, cancelArgs) =>
			{
				cancelArgs.Cancel = shouldCancel;
			};

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Delete(insertedAggregate));

			// Assert
			Assert.NotNull(repository.Get(insertedAggregate));

			// Cleanup
			shouldCancel = false;
			repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Delete_PostDeleteHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			Category? deletedAggregate = null;
			repository.PostDelete += (tmpAggregate) =>
			{
				deletedAggregate = tmpAggregate;
			};

			// Act
			var result = repository.Delete(insertedAggregate);

			// Assert
			Assert.Equal(result, deletedAggregate);
			Assert.Same(result, deletedAggregate);
		}

		[Theory, AutoDomainData]
		public void Delete_PostDeleteThrows_IsDeleted(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			repository.PostDelete += (tmpAggregate) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var result = repository.Delete(insertedAggregate);

			// Assert
			Assert.True(result?.CategoryId > 0);
			var gotten = repository.Get(result!);
			Assert.Null(gotten);
		}
		#endregion

		#region Insert
		[Theory, AutoDomainData]
		public void Insert_PreInsertHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertAggregate, cancelArgs) =>
			{
				// Assert
				Assert.Equal(aggregate, preInsertAggregate);
			};

			// Act
			var insertedAggregate = repository.Insert(aggregate);

			repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Insert_PreInsertThrows_IsInserted(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertAggregate, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedAggregate = repository.Insert(aggregate);

			// Assert
			try
			{
				Assert.True(insertedAggregate.CategoryId > 0);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_PreInsertCancels_IsCancelled(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreInsert += (preInsertAggregate, cancelArgs) =>
			{
				cancelArgs.Cancel = true;
			};

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Insert(aggregate));

			var allNames = repository.GetAll().Select(category => category.Name);
			Assert.DoesNotContain(aggregate.Name, allNames);
		}

		[Theory, AutoDomainData]
		public void Insert_PostInsertHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			Category? postInsertAggregate = null;
			repository.PostInsert += (tmpAggregate) =>
			{
				postInsertAggregate = tmpAggregate;
			};

			// Act
			var insertedAggregate = repository.Insert(aggregate);

			// Assert
			try
			{
				Assert.Equal(insertedAggregate, postInsertAggregate);
				Assert.Same(insertedAggregate, postInsertAggregate);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_PostInsertThrows_IsInserted(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PostInsert += (tmpAggregate) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var insertedAggregate = repository.Insert(aggregate);

			// Assert
			try
			{
				Assert.True(insertedAggregate.CategoryId > 0);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}
		#endregion

		#region Update
		[Theory, AutoDomainData]
		public void Update_PreUpdateHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			var insertedAggregate = repository.Insert(aggregate);

			var aggregateToUpdate = insertedAggregate with { Description = "Hello world" };

			repository.PreUpdate += (preUpdateAggregate, cancelArgs) =>
			{
				// Assert
				Assert.Equal(aggregateToUpdate, preUpdateAggregate);
			};

			// Act
			repository.Update(aggregateToUpdate);

			repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Update_PreUpdateThrows_IsUpdated(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();
			var insertedAggregate = repository.Insert(aggregate);

			repository.PreUpdate += (preUpdateAggregate, cancelArgs) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var updatedAggregate = repository.Update(insertedAggregate with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal("Hello world", updatedAggregate?.Description);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}

		[Theory, AutoDomainData]
		public void Update_PreUpdateCancels_IsCancelled(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			repository.PreUpdate += (preUpdateAggregate, cancelArgs) =>
			{
				cancelArgs.Cancel = true;
			};

			var insertedAggregate = repository.Insert(aggregate);

			// Act && Assert
			Assert.Throws<CanceledException>(() => repository.Update(insertedAggregate with { Description = "Hello world" }));
			var gottenAggregate = repository.Get(insertedAggregate);
			Assert.Equal(aggregate.Description, gottenAggregate?.Description);

			repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Update_PostUpdateHasEvent_IsInvoked(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			Category? postUpdateAggregate = null;
			repository.PostUpdate += (tmpAggregate) =>
			{
				postUpdateAggregate = tmpAggregate;
			};

			// Act
			var insertedAggregate = repository.Insert(aggregate);

			var updatedAggregate = repository.Update(insertedAggregate with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal(updatedAggregate, postUpdateAggregate);
				Assert.Same(updatedAggregate, postUpdateAggregate);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}

		[Theory, AutoDomainData]
		public void Update_PostUpdatedThrows_IsUpdated(Category aggregate)
		{
			// Arrange
			var repository = new CategoryRepository();

			var insertedAggregate = repository.Insert(aggregate);

			repository.PostUpdate += (tmpAggregate) =>
			{
				throw new InvalidOperationException();
			};

			// Act
			var updatedAggregate = repository.Update(insertedAggregate with { Description = "Hello world" });

			// Assert
			try
			{
				Assert.Equal("Hello world", updatedAggregate?.Description);
			}
			finally
			{
				repository.Delete(insertedAggregate);
			}
		}
		#endregion
	}
}
