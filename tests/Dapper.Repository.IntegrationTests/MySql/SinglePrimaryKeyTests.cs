using System;
using System.Linq;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using MySql.Data.MySqlClient;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
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
		public void Delete_PrimaryKeyNotEntered_Throws(Category aggregate)
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Delete(aggregate));
		}

		[Fact]
		public void Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act 
			var deleted = _repository.Delete(new CategoryPrimaryKeyAggregate { CategoryId = int.MaxValue });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public void Delete_UsePrimaryKey_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var deleted = _repository.Delete(new CategoryPrimaryKeyAggregate { CategoryId = insertedAggregate.CategoryId });

			// Assert
			Assert.Equal(insertedAggregate.CategoryId, deleted?.CategoryId);
			Assert.Equal(aggregate.Description, deleted?.Description);
			Assert.Equal(aggregate.Name, deleted?.Name);
			Assert.Equal(aggregate.Picture, deleted?.Picture);
		}

		[Theory, AutoDomainData]
		public void Delete_UseAggregate_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var deleted = _repository.Delete(insertedAggregate);

			// Assert
			Assert.Equal(insertedAggregate.CategoryId, deleted?.CategoryId);
			Assert.Equal(aggregate.Description, deleted?.Description);
			Assert.Equal(aggregate.Name, deleted?.Name);
			Assert.Equal(aggregate.Picture, deleted?.Picture);
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
		public void Get_UsePrimaryKey_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var fetchedAggregate = _repository.Get(new CategoryPrimaryKeyAggregate { CategoryId = insertedAggregate.CategoryId });

			// Assert
			Assert.Equal(insertedAggregate.Name, fetchedAggregate?.Name);
			Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
			Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

			_repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Get_UseFullAggregate_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var fetchedAggregate = _repository.Get(insertedAggregate);

			// Assert
			Assert.Equal(insertedAggregate.Description, fetchedAggregate?.Description);
			Assert.Equal(insertedAggregate.Name, fetchedAggregate?.Name);
			Assert.Equal(insertedAggregate.Picture, fetchedAggregate?.Picture);

			_repository.Delete(insertedAggregate);
		}

		[Fact]
		public void Get_PrimaryKeyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Get(new CategoryPrimaryKeyAggregate { }));
		}

		[Fact]
		public void Get_UseMissingPrimaryKey_ReturnsNull()
		{
			// Act
			var gotten = _repository.Get(new CategoryPrimaryKeyAggregate { CategoryId = int.MaxValue });

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
		public void Insert_HasIdaggregateKeyWithValue_Throws()
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
			Assert.Throws<ArgumentException>(() => _repository.Insert(aggregate));
		}

		[Theory, AutoDomainData]
		public void Insert_HasIdaggregateKeyWithoutValue_IsInserted(Category aggregate)
		{
			// Act
			var insertedAggregate = _repository.Insert(aggregate);
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
				_repository.Delete(insertedAggregate);
			}
		}

		[Fact]
		public void Insert_NonNullPropertyMissing_Throws()
		{
			// Arrange
			var aggregate = new Category
			{
				Description = "Lorem ipsum, dolor sit amit",
				Name = null!,
				Picture = null
			};

			// Act && Assert
			Assert.Throws<MySqlException>(() => _repository.Insert(aggregate));
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
		public void Update_UseAggregate_Valid(Category aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			var update = insertedAggregate with { Description = "Something else" };

			// Act
			var updatedAggregate = _repository.Update(update);

			// Assert
			Assert.Equal("Something else", updatedAggregate?.Description);

			_repository.Delete(insertedAggregate);
		}

		[Theory, AutoDomainData]
		public void Update_PrimaryKeyNotEntered_Throws(Category aggregate)
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Update(aggregate));
		}

		[Fact]
		public void Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Arrange
			var aggregate = new Category
			{
				CategoryId = int.MaxValue,
				Description = "Lorem ipsum, dolor sit amit",
				Name = "Hello world"
			};

			// Act
			var updated = _repository.Update(aggregate);

			// Assert
			Assert.Null(updated);
		}
		#endregion
	}
}
