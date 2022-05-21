using Dapper.Repository.BaseRepositories;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.Sql
{
	/// <summary>
	/// Provides a repository for "heap" tables (tables without a primary key)
	/// </summary>
	public abstract class HeapRepository<TEntity> : BaseHeapRepository<TEntity>, IHeapRepository<TEntity>
	where TEntity : DbEntity
	{
		protected virtual string Schema { get; } = "dbo";

		protected override string FormattedTableName => $"[{Schema}].[{TableName}]";

		protected override IQueryGenerator<TEntity> CreateQueryGenerator()
		{
			return new SqlQueryGenerator<TEntity>(Schema, TableName);
		}

		protected HeapRepository() : base()
		{
		}
	}
}