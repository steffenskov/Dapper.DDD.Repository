using System;
using System.Threading.Tasks;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class CompositePrimaryKeyAsyncTests
	{
		private readonly CompositeUserRepository _repository;

		public CompositePrimaryKeyAsyncTests()
		{
			_repository = new CompositeUserRepository();
		}

		[Fact]
		public async Task Delete_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(new CompositeUserPrimaryKeyAggregate { Username = "async My name" }));
		}

		[Fact]
		public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = await _repository.DeleteAsync(new CompositeUserPrimaryKeyAggregate { Username = "async My name", Password = "Secret" });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var deleted = await _repository.DeleteAsync(new CompositeUserPrimaryKeyAggregate { Username = aggregate.Username, Password = aggregate.Password });

			// Assert
			Assert.Equal(aggregate.Username, deleted?.Username);
			Assert.Equal(aggregate.Password, deleted?.Password);
			Assert.Equal(insertedAggregate.DateCreated, deleted?.DateCreated);
		}

		[Fact]
		public async Task Get_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.GetAsync(new CompositeUserPrimaryKeyAggregate { Username = "async My name" }));
		}

		[Fact]
		public async Task Get_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var gotten = await _repository.GetAsync(new CompositeUserPrimaryKeyAggregate { Username = "async My name", Password = "Secret" });

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public async Task Get_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
		{
			// Arrange
			var insertedAggregate = _repository.Insert(aggregate);

			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyAggregate { Username = aggregate.Username, Password = aggregate.Password });

			// Assert
			Assert.Equal(aggregate.Username, gotten?.Username);
			Assert.Equal(aggregate.Password, gotten?.Password);
			Assert.Equal(insertedAggregate.DateCreated, gotten?.DateCreated);

			await _repository.DeleteAsync(insertedAggregate);
		}

		[Fact]
		public async Task Update_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.UpdateAsync(new CompositeUserAggregate { Username = "async My name" }));
		}

		[Fact]
		public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var updated = await _repository.UpdateAsync(new CompositeUserAggregate { Username = "Doesnt exist", Password = "Secret" });

			// Assert
			Assert.Null(updated);
		}

		[Theory, AutoDomainData]
		public async Task Update_UsePrimaryKey_Valid(CompositeUserAggregate aggregate)
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

			await _repository.DeleteAsync(insertedAggregate);
		}
	}
}