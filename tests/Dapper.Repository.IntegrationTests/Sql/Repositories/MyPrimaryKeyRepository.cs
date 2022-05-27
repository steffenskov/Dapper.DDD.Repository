using System.Data;
using Dapper.Repository.Sql;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public abstract class MyPrimaryKeyRepository<TPrimaryKeyAggregate, TAggregate> : PrimaryKeyRepository<TPrimaryKeyAggregate, TAggregate>
	where TPrimaryKeyAggregate
	where TAggregate : TPrimaryKeyAggregate
	{
		protected override IDbConnection CreateConnection()
		{
			return ConnectionFactory.CreateSqlConnection();
		}

		protected override IDapperInjection<T> CreateDapperInjection<T>()
		{
			return new DapperInjection<T>();
		}
	}
}
