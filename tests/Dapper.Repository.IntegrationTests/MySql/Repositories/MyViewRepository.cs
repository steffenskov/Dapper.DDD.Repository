using System;
using System.Data;
using Dapper.Repository.MySql;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public abstract class MyViewRepository<TEntity> : ViewRepository<TEntity>
	where TEntity : DbEntity
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
