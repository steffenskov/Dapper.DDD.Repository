using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Repository.Delegates;
using Dapper.Repository.Exceptions;
using Dapper.Repository.Interfaces;
using Dapper.Repository.MetaInformation;

namespace Dapper.Repository.BaseRepositories
{
	public abstract class BaseHeapRepository<TEntity> : BaseDbRepository<TEntity>, IHeapRepository<TEntity>
	where TEntity : DbEntity
	{
		protected abstract string TableName { get; }

		protected virtual string FormattedTableName => TableName;

		#region Events

		public event PreOperationDelegate<TEntity>? PreInsert;
		public event PostOperationDelegate<TEntity>? PostInsert;
		public event PreOperationDelegate<TEntity>? PreDelete;
		public event PostOperationDelegate<TEntity?>? PostDelete;
		#endregion

		public BaseHeapRepository() : base()
		{
		}

		#region Delete
		public TEntity? Delete(TEntity entity)
		{
			InvokePreOperation(PreDelete, entity);
			var result = DeleteInternalAsync(entity, (query, input) => Task.FromResult(Query(query, input)))
							.GetAwaiter()
							.GetResult(); // This is safe because we're using Task.FromResult as the only "async" part

			InvokePostOperation(PostDelete, result);
			return result;
		}

		public async Task<TEntity?> DeleteAsync(TEntity entity)
		{
			InvokePreOperation(PreDelete, entity);
			var result = await DeleteInternalAsync(entity, async (query, input) => await QueryAsync(query, input));
			InvokePostOperation(PostDelete, result);
			return result;
		}

		private async Task<TEntity?> DeleteInternalAsync(TEntity entity, Func<string, TEntity, Task<IEnumerable<TEntity>>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			var query = _queryGenerator.GenerateDeleteQuery();
			var result = await execute(query, entity);

			return result?.FirstOrDefault();
		}
		#endregion

		#region Get
		public TEntity? Get(TEntity entity)
		{
			return GetInternalAsync(entity, (query, input) => Task.FromResult(Query(query, input)))
						.GetAwaiter()
						.GetResult(); // This is safe because we're using Task.FromResult as the only "async" part
		}

		public async Task<TEntity?> GetAsync(TEntity entity)
		{
			return await GetInternalAsync(entity, async (query, input) => await QueryAsync(query, input));
		}

		private async Task<TEntity?> GetInternalAsync(TEntity entity, Func<string, TEntity, Task<IEnumerable<TEntity>>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			var query = _queryGenerator.GenerateGetQuery();
			var result = await execute(query, entity);

			return result?.FirstOrDefault();
		}
		#endregion

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

		#region Insert
		public TEntity Insert(TEntity entity)
		{
			InvokePreOperation(PreInsert, entity);
			var result = InsertInternalAsync(entity, (query, input) => Task.FromResult(Query(query, input).First()))
							.GetAwaiter()
							.GetResult();
			InvokePostOperationNotNullable(PostInsert, result);
			return result;
		}

		public async Task<TEntity> InsertAsync(TEntity entity)
		{
			InvokePreOperation(PreInsert, entity);
			var result = await InsertInternalAsync(entity, async (query, input) => (await QueryAsync(query, input)).First());
			InvokePostOperationNotNullable(PostInsert, result);
			return result;
		}

		private async Task<TEntity> InsertInternalAsync(TEntity entity, Func<string, TEntity, Task<TEntity>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var query = _queryGenerator.GenerateInsertQuery(entity);
			return await execute(query, entity);
		}

		#endregion

		#region Event invokation
		protected void InvokePreOperation(PreOperationDelegate<TEntity>? @delegate, TEntity entity)
		{
			var cancelArg = new CancelEventArgs();
			try
			{
				@delegate?.Invoke(entity, cancelArg);
			}
			catch { }
			if (cancelArg.Cancel)
			{
				throw new CanceledException("Cancelled by event");
			}
		}

		protected void InvokePostOperation(PostOperationDelegate<TEntity?>? @delegate, TEntity? entity)
		{
			try
			{
				@delegate?.Invoke(entity);
			}
			catch { }
		}

		protected void InvokePostOperationNotNullable(PostOperationDelegate<TEntity>? @delegate, TEntity entity)
		{
			try
			{
				@delegate?.Invoke(entity);
			}
			catch { }
		}
		#endregion
	}
}