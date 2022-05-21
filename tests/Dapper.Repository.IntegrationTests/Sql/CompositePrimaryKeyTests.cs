using System;
using Dapper.Repository.IntegrationTests.Entities;
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
			Assert.Throws<ArgumentException>(() => _repository.Delete(new CompositeUserPrimaryKeyEntity { Username = "My name" }));
		}

		[Fact]
		public void Delete_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var deleted = _repository.Delete(new CompositeUserPrimaryKeyEntity { Username = "My name", Password = "Secret" });

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public void Delete_UsePrimaryKey_Valid(CompositeUserEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var deleted = _repository.Delete(new CompositeUserPrimaryKeyEntity { Username = entity.Username, Password = entity.Password });

			// Assert
			Assert.Equal(entity.Username, deleted?.Username);
			Assert.Equal(entity.Password, deleted?.Password);
			Assert.Equal(insertedEntity.DateCreated, deleted?.DateCreated);
		}

		[Fact]
		public void Get_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Get(new CompositeUserPrimaryKeyEntity { Username = "My name" }));
		}

		[Fact]
		public void Get_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyEntity { Username = "My name", Password = "Secret" });

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public void Get_UsePrimaryKey_Valid(CompositeUserEntity entity)
		{
			// Arrange
			var insertedEntity = _repository.Insert(entity);

			// Act
			var gotten = _repository.Get(new CompositeUserPrimaryKeyEntity { Username = entity.Username, Password = entity.Password });

			// Assert
			Assert.Equal(entity.Username, gotten?.Username);
			Assert.Equal(entity.Password, gotten?.Password);
			Assert.Equal(insertedEntity.DateCreated, gotten?.DateCreated);

			_repository.Delete(insertedEntity);
		}

		[Fact]
		public void Update_PrimaryKeyPartiallyNotEntered_Throws()
		{
			// Act && Assert
			Assert.Throws<ArgumentException>(() => _repository.Update(new CompositeUserEntity { Username = "My name" }));
		}

		[Fact]
		public void Update_UseMissingPrimaryKeyValue_ReturnsNull()
		{
			// Act && Assert
			var updated = _repository.Update(new CompositeUserEntity { Username = "Doesnt exist", Password = "Secret" });

			// Assert
			Assert.Null(updated);
		}

		[Theory, AutoDomainData]
		public void Update_UsePrimaryKey_Valid(CompositeUserEntity entity)
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

			_repository.Delete(insertedEntity);
		}
	}
}