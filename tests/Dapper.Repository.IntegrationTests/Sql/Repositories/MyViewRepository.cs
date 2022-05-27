using System;
using System.Data;
using Dapper.Repository.Sql;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public abstract class MyViewRepository<TAggregate> : ViewRepository<TAggregate>
	where TAggregate
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
