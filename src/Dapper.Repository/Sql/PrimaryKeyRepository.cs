using Dapper.Repository.BaseRepositories;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.Sql
{
	/// <summary>
	/// Provides a repository for tables with a primary key defined (either single column or composite)
	/// </summary>
	public abstract class PrimaryKeyRepository<TPrimaryKeyEntity, TEntity> : BasePrimaryKeyRepository<TPrimaryKeyEntity, TEntity>, IRepository<TPrimaryKeyEntity, TEntity>
	where TPrimaryKeyEntity : DbEntity
	where TEntity : TPrimaryKeyEntity
	{
		protected virtual string Schema { get; } = "dbo";

		protected override string FormattedTableName => $"[{Schema}].[{TableName}]";

		protected override IQueryGenerator<TEntity> CreateQueryGenerator()
		{
			return new SqlQueryGenerator<TEntity>(Schema, TableName);
		}


		public PrimaryKeyRepository() : base()
		{

		}
	}
}
