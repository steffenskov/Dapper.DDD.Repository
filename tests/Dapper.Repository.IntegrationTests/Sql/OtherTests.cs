using System;
using Dapper.Repository.IntegrationTests.Aggregates;
using Dapper.Repository.IntegrationTests.Sql.Repositories;
using Xunit;

namespace Dapper.Repository.IntegrationTests.Sql
{
	public class OtherTests
	{
		private readonly CompositeUserRepository _repository;

		public OtherTests()
		{
			_repository = new CompositeUserRepository();
		}

		[Theory, AutoDomainData]
		public void Insert_RelyOnDefaultConstraint_Valid(CompositeUserAggregate aggregate)
		{
			// Act
			var insertedAggregate = _repository.Insert(aggregate);

			// Assert
			try
			{
				Assert.Equal(aggregate.Username, insertedAggregate.Username);
				Assert.Equal(aggregate.Password, insertedAggregate.Password);
				Assert.True(insertedAggregate.DateCreated > DateTime.UtcNow.AddHours(-1));
			}
			finally
			{
				_repository.Delete(aggregate);
			}
		}

		[Theory, AutoDomainData]
		public void Update_ColumnHasMissingSetter_ColumnIsExcluded(CompositeUserAggregate aggregate)
		{
			// Act
			var insertedAggregate = _repository.Insert(aggregate);

			// Assert
			try
			{
				Assert.Equal(aggregate.Username, insertedAggregate.Username);
				Assert.Equal(aggregate.Password, insertedAggregate.Password);
				Assert.True(insertedAggregate.DateCreated > DateTime.UtcNow.AddHours(-1));
			}
			finally
			{
				_repository.Delete(aggregate);
			}
		}
	}
}