using Dapper.Repository.BaseRepositories;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.MySql
{
	public abstract class ViewRepository<TEntity> : BaseViewRepository<TEntity>, IViewRepository<TEntity>
	where TEntity : DbEntity
	{
		protected override IQueryGenerator<TEntity> CreateQueryGenerator()
		{
			return new MySqlQueryGenerator<TEntity>(ViewName);
		}

		public ViewRepository()
		{
		}
	}
}
