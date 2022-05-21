using System.Data;
using Dapper.Repository.Sql;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public abstract class MyPrimaryKeyRepository<TPrimaryKeyEntity, TEntity> : PrimaryKeyRepository<TPrimaryKeyEntity, TEntity>
	where TPrimaryKeyEntity : DbEntity
	where TEntity : TPrimaryKeyEntity
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
