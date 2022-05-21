using System.Data;
using Dapper.Repository.Interfaces;
using Dapper.Repository.MySql;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public abstract class MyHeapRepository<TEntity> : HeapRepository<TEntity>
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
