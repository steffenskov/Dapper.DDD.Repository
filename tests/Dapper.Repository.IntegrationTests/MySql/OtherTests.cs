using System;
using Dapper.Repository.IntegrationTests.Entities;
using Dapper.Repository.IntegrationTests.MySql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.MySql
{
	public class OtherTests
	{
		private readonly CompositeUserRepository _repository;

		public OtherTests()
		{
			_repository = new CompositeUserRepository();
		}

		[Theory, AutoDomainData]
		public void Insert_RelyOnDefaultConstraint_Valid(CompositeUserEntity entity)
		{
			// Act
			var insertedEntity = _repository.Insert(entity);

			// Assert
			try
			{
				Assert.Equal(entity.Username, insertedEntity.Username);
				Assert.Equal(entity.Password, insertedEntity.Password);
				Assert.True(insertedEntity.DateCreated > DateTime.UtcNow.AddHours(-1));
			}
			finally
			{
				_repository.Delete(entity);
			}
		}

		[Theory, AutoDomainData]
		public void Update_ColumnHasMissingSetter_ColumnIsExcluded(CompositeUserEntity entity)
		{
			// Act
			var insertedEntity = _repository.Insert(entity);

			// Assert
			try
			{
				Assert.Equal(entity.Username, insertedEntity.Username);
				Assert.Equal(entity.Password, insertedEntity.Password);
				Assert.True(insertedEntity.DateCreated > DateTime.UtcNow.AddHours(-1));
			}
			finally
			{
				_repository.Delete(entity);
			}
		}
	}
}