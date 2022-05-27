using System.Data;
using Dapper.Repository.MySql;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public abstract class MyPrimaryKeyRepository<TPrimaryKeyAggregate, TAggregate> : PrimaryKeyRepository<TPrimaryKeyAggregate, TAggregate>
	where TPrimaryKeyAggregate
	where TAggregate : TPrimaryKeyAggregate
	{
		protected override IDbConnection CreateConnection()
		{
			return ConnectionFactory.CreateMySqlConnection();
		}

		protected override IDapperInjection<T> CreateDapperInjection<T>()
		{
			return new DapperInjection<T>();
		}
	}
}
