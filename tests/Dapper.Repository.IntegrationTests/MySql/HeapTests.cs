using System.Linq;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using MySql.Data.MySqlClient;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
{
	public class HeapTests
	{
		private readonly UserHeapRepository _repository;

		public HeapTests()
		{
			_repository = new UserHeapRepository();
		}

		[Fact]
		public void Delete_MissingColumns_ReturnsNull()
		{
			// Arrange
			var entity = new UserHeapEntity { Username = "async My name" };

			// Act
			var deleted = _repository.Delete(entity);

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public void Delete_HasAllColumns_Valid(UserHeapEntity entity)
		{
			// Arrange
			_repository.Insert(entity);

			// Act
			var deletedEntity = _repository.Delete(entity);

			// Assert
			Assert.Equal(entity.Username, deletedEntity?.Username);
			Assert.Equal(entity.Password, deletedEntity?.Password);
		}

		[Theory, AutoDomainData]
		public void Delete_MultipleRowsWithSameValues_DeletesBoth(UserHeapEntity entity)
		{
			// Arrange
			_repository.Insert(entity);
			_repository.Insert(entity);

			// Act
			var deletedEntity = _repository.Delete(entity);

			// Assert
			var gotten = _repository.Get(entity);
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public void Get_ValuesNotInDatabase_ReturnsNull(UserHeapEntity entity)
		{
			// Act
			var gotten = _repository.Get(entity);

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public void Get_MultipleRowsWithSameValues_Valid(UserHeapEntity entity)
		{
			// Arrange
			_repository.Insert(entity);
			_repository.Insert(entity);

			// Act
			var gotten = _repository.Get(entity);

			try
			{
				// Assert
				Assert.Equal(entity.Username, gotten?.Username);
				Assert.Equal(entity.Password, gotten?.Password);
			}
			finally
			{
				_repository.Delete(entity);
			}
		}

		[Theory, AutoDomainData]
		public void GetAll_HaveRows_Valid(UserHeapEntity entity, UserHeapEntity entity2)
		{
			// Arrange
			_repository.Insert(entity);
			_repository.Insert(entity2);

			// Act
			var all = _repository.GetAll();

			try
			{
				Assert.True(all.Count() >= 2);
			}
			finally
			{
				_repository.Delete(entity);
				_repository.Delete(entity2);
			}
		}

		[Fact]
		public void Insert_MissingColumns_Throws()
		{
			// Arrange
			var entity = new UserHeapEntity { Username = "async My name" };

			// Act && Assert
			Assert.Throws<MySqlException>(() => _repository.Insert(entity));
		}

		[Theory, AutoDomainData]
		public void Insert_HasAllColumns_Valid(UserHeapEntity entity)
		{
			// Act
			var insertedEntity = _repository.Insert(entity);
			try
			{
				// Assert
				Assert.Equal(entity.Username, insertedEntity.Username);
				Assert.Equal(entity.Password, insertedEntity.Password);
			}
			finally
			{
				_repository.Delete(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public void Insert_InsertSameValuesTwice_BothAreCreated(UserHeapEntity entity)
		{
			// Act
			_repository.Insert(entity);
			_repository.Insert(entity);
			try
			{
				// Assert
				var count = _repository.GetAll()
										.Where(found => found.Username == entity.Username && found.Password == entity.Password)
										.Count();
				Assert.True(count > 1);
			}
			finally
			{
				_repository.Delete(entity);
			}
		}
	}
}