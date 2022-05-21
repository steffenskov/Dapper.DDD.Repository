using System;
using System.Threading.Tasks;
using Dapper.Repository.IntegrationTests.Entities;
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
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(new CompositeUserPrimaryKeyEntity { Username = "async My name" }));
		}

		[Fact]
		public async Task Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = await _repository.DeleteAsync(new CompositeUserPrimaryKeyEntity { Username = "async My name", Password = "Secret" });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public async Task Delete_UsePrimaryKey_Valid(CompositeUserEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var deleted = await _repository.DeleteAsync(new CompositeUserPrimaryKeyEntity { Username = entity.Username, Password = entity.Password });

			// Assert
			Assert.Equal(entity.Username, deleted?.Username);
			Assert.Equal(entity.Password, deleted?.Password);
			Assert.Equal(insertedEntity.DateCreated, deleted?.DateCreated);
		}

		[Fact]
		public async Task Get_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.GetAsync(new CompositeUserPrimaryKeyEntity { Username = "async My name" }));
		}

		[Fact]
		public async Task Get_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var gotten = await _repository.GetAsync(new CompositeUserPrimaryKeyEntity { Username = "async My name", Password = "Secret" });

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public async Task Get_UsePrimaryKey_Valid(CompositeUserEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyEntity { Username = entity.Username, Password = entity.Password });

			// Assert
			Assert.Equal(entity.Username, gotten?.Username);
			Assert.Equal(entity.Password, gotten?.Password);
			Assert.Equal(insertedEntity.DateCreated, gotten?.DateCreated);

			await _repository.DeleteAsync(insertedEntity);
		}

		[Fact]
		public async Task Update_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.UpdateAsync(new CompositeUserEntity { Username = "async My name" }));
		}

		[Fact]
		public async Task Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var updated = await _repository.UpdateAsync(new CompositeUserEntity { Username = "Doesnt exist", Password = "Secret" });

			// Assert
			Assert.Null(updated);
		}

		[Theory, AutoDomainData]
		public async Task Update_UsePrimaryKey_Valid(CompositeUserEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var updated = _repository.Update(insertedEntity with { Age = 42 });

			// Assert
			Assert.Equal(entity.Username, updated?.Username);
			Assert.Equal(entity.Password, updated?.Password);
			Assert.NotEqual(42, insertedEntity.Age);
			Assert.Equal(42, updated?.Age);
			Assert.Equal(insertedEntity.DateCreated, updated?.DateCreated);

			await _repository.DeleteAsync(insertedEntity);
		}
	}
}