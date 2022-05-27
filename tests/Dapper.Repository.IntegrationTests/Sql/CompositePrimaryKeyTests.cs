using System;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class CompositePrimaryKeyTests
	{
		private readonly CompositeUserRepository _repository;

		public CompositePrimaryKeyTests()
		{
			_repository = new CompositeUserRepository();
		}

		[Fact]
		public void Delete_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Delete(new CompositeUserPrimaryKeyAggregate { Username = "My name" }));
		}

		[Fact]
		public void Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = _repository.Delete(new CompositeUserPrimaryKeyAggregate { Username = "My name", Password = "Secret" });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public void Delete_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var deleted = _repository.Delete(new CompositeUserPrimaryKeyAggregate { Username = aggregate.Username, Password = aggregate.Password });

			// Assert
			Assert.Equal(aggregate.Username, deleted?.Username);
			Assert.Equal(aggregate.Password, deleted?.Password);
			Assert.Equal(insertedAggregate.DateCreated, deleted?.DateCreated);
		}

		[Fact]
		public void Get_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Get(new CompositeUserPrimaryKeyAggregate { Username = "My name" }));
		}

		[Fact]
		public void Get_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyAggregate { Username = "My name", Password = "Secret" });

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public void Get_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyAggregate { Username = aggregate.Username, Password = aggregate.Password });

			// Assert
			Assert.Equal(aggregate.Username, gotten?.Username);
			Assert.Equal(aggregate.Password, gotten?.Password);
			Assert.Equal(insertedAggregate.DateCreated, gotten?.DateCreated);

			_repository.Delete(insertedAggregate);
		}

		[Fact]
		public void Update_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Update(new CompositeUserAggregate { Username = "My name" }));
		}

		[Fact]
		public void Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act && Assert
			var updated = _repository.Update(new CompositeUserAggregate { Username = "Doesnt exist", Password = "Secret" });

			// Assert
			Assert.Null(updated);
		}

		[Theory, AutoDomainData]
		public void Update_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var updated = _repository.Update(insertedAggregate with { Age = 42 });

			// Assert
			Assert.Equal(aggregate.Username, updated?.Username);
			Assert.Equal(aggregate.Password, updated?.Password);
			Assert.NotEqual(42, insertedAggregate.Age);
			Assert.Equal(42, updated?.Age);
			Assert.Equal(insertedAggregate.DateCreated, updated?.DateCreated);

			_repository.Delete(insertedAggregate);
		}
	}
}