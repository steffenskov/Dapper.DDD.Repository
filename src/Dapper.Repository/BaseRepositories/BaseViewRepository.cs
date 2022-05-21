using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.BaseRepositories
{
	public abstract class BaseViewRepository<TEntity> : BaseDbRepository<TEntity>, IViewRepository<TEntity>
	where TEntity : DbEntity
	{
		protected abstract string ViewName { get; }

		protected virtual string FormattedViewName => ViewName;

		protected BaseViewRepository() : base()
		{
		}

		#region GetAll
		public IEnumerable<TEntity> GetAll()
		{
			return GetAllInternalAsync((query) => Task.FromResult(Query(query)))
						.GetAwaiter()
						.GetResult(); // This is safe because we're using Task.FromResult as the only "async" part
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await GetAllInternalAsync(async (query) => await QueryAsync(query));
		}

		private async Task<IEnumerable<TEntity>> GetAllInternalAsync(Func<string, Task<IEnumerable<TEntity>>> execute)
		{
			var query = _queryGenerator.GenerateGetAllQuery();
			return await execute(query);
		}
		#endregion
	}
}
