using System.Linq;
using System.Threading.Tasks;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using MySql.Data.MySqlClient;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
{
	public class HeapAsyncTests
	{
		private readonly UserHeapRepository _repository;

		public HeapAsyncTests()
		{
			_repository = new UserHeapRepository();
		}

		[Fact]
		public async Task Delete_MissingColumns_ReturnsNull()
		{
			// Arrange
			var entity = new UserHeapEntity { Username = "My name" };

			// Act 
			var deleted = await _repository.DeleteAsync(entity);

			// Assert
			Assert.Null(deleted);
		}

		[Theory, AutoDomainData]
		public async Task Delete_HasAllColumns_Valid(UserHeapEntity entity)
		{
			// Arrange
			await _repository.InsertAsync(entity);

			// Act
			var deletedEntity = await _repository.DeleteAsync(entity);

			// Assert
			Assert.Equal(entity.Username, deletedEntity?.Username);
			Assert.Equal(entity.Password, deletedEntity?.Password);
		}

		[Theory, AutoDomainData]
		public async Task Delete_MultipleRowsWithSameValues_DeletesBoth(UserHeapEntity entity)
		{
			// Arrange
			await _repository.InsertAsync(entity);
			await _repository.InsertAsync(entity);

			// Act
			var deletedEntity = await _repository.DeleteAsync(entity);

			// Assert
			var gotten = await _repository.GetAsync(entity);
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public async Task Get_ValuesNotInDatabase_ReturnsNull(UserHeapEntity entity)
		{
			// Act
			var gotten = await _repository.GetAsync(entity);

			// Assert
			Assert.Null(gotten);
		}

		[Theory, AutoDomainData]
		public async Task Get_MultipleRowsWithSameValues_Valid(UserHeapEntity entity)
		{
			// Arrange
			await _repository.InsertAsync(entity);
			await _repository.InsertAsync(entity);

			// Act
			var gotten = await _repository.GetAsync(entity);

			try
			{
				// Assert
				Assert.Equal(entity.Username, gotten?.Username);
				Assert.Equal(entity.Password, gotten?.Password);
			}
			finally
			{
				await _repository.DeleteAsync(entity);
			}
		}

		[Theory, AutoDomainData]
		public async Task GetAll_HaveRows_Valid(UserHeapEntity entity)
		{
			// Arrange
			await _repository.InsertAsync(entity);

			// Act
			var deletedEntity = await _repository.DeleteAsync(entity);

			// Assert
			Assert.Equal(entity.Username, deletedEntity?.Username);
			Assert.Equal(entity.Password, deletedEntity?.Password);
		}

		[Fact]
		public async Task Insert_MissingColumns_Throws()
		{
			// Arrange
			var entity = new UserHeapEntity { Username = "My name" };

			// Act && Assert
			await Assert.ThrowsAsync<MySqlException>(async () => await _repository.InsertAsync(entity));
		}

		[Theory, AutoDomainData]
		public async Task Insert_HasAllColumns_Valid(UserHeapEntity entity)
		{
			// Act
			var insertedEntity = await _repository.InsertAsync(entity);
			try
			{
				// Assert
				Assert.Equal(entity.Username, insertedEntity.Username);
				Assert.Equal(entity.Password, insertedEntity.Password);
			}
			finally
			{
				await _repository.DeleteAsync(insertedEntity);
			}
		}

		[Theory, AutoDomainData]
		public async Task Insert_InsertSameValuesTwice_BothAreCreated(UserHeapEntity entity)
		{
			// Act
			await _repository.InsertAsync(entity);
			await _repository.InsertAsync(entity);
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
				await _repository.DeleteAsync(entity);
			}
		}
	}
}