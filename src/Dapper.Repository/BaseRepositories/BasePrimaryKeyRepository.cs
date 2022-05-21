using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Repository.Delegates;
using Dapper.Repository.Exceptions;
using Dapper.Repository.Interfaces;
using Dapper.Repository.MetaInformation;

namespace Dapper.Repository.BaseRepositories
{
	/// <summary>
	/// Provides a repository for tables with a primary key defined (either single column or composite)
	/// </summary>
	public abstract class BasePrimaryKeyRepository<TPrimaryKeyEntity, TEntity> : BaseDbRepository<TEntity>, IRepository<TPrimaryKeyEntity, TEntity>
	where TPrimaryKeyEntity : DbEntity
	where TEntity : TPrimaryKeyEntity
	{
		protected abstract string TableName { get; }

		protected virtual string FormattedTableName => TableName;

		#region Events
		public event PreOperationDelegate<TEntity>? PreInsert;
		public event PostOperationDelegate<TEntity>? PostInsert;
		public event PreOperationDelegate<TPrimaryKeyEntity>? PreDelete;
		public event PostOperationDelegate<TEntity?>? PostDelete;
		public event PreOperationDelegate<TEntity>? PreUpdate;
		public event PostOperationDelegate<TEntity?>? PostUpdate;
		#endregion

		protected BasePrimaryKeyRepository() : base()
		{
		}

		#region Delete
		public TEntity? Delete(TPrimaryKeyEntity entity)
		{
			InvokePreOperation(PreDelete, entity);

			var result = DeleteInternalAsync(entity, (pk, query) => Task.FromResult(QuerySingleOrDefault(query, pk)))
							.GetAwaiter()
							.GetResult(); // This is safe because we're using Task.FromResult as the only "async" part
			InvokePostOperation(PostDelete, result);
			return result;
		}

		public async Task<TEntity?> DeleteAsync(TPrimaryKeyEntity entity)
		{
			InvokePreOperation(PreDelete, entity);
			var result = await DeleteInternalAsync(entity, async (pk, query) => await QuerySingleOrDefaultAsync(query, pk));
			InvokePostOperation(PostDelete, result);
			return result;
		}

		private async Task<TEntity?> DeleteInternalAsync(TPrimaryKeyEntity entity, Func<TPrimaryKeyEntity, string, Task<TEntity?>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			CheckForDefaultPrimaryKeys(info, entity);

			var query = _queryGenerator.GenerateDeleteQuery();
			var result = await execute(entity, query);

			return result;
		}
		#endregion

		#region Get
		public TEntity? Get(TPrimaryKeyEntity entity)
		{
			return GetInternal(entity, (pk, query) => Task.FromResult(QuerySingleOrDefault(query, pk)))
					.GetAwaiter()
					.GetResult(); // This is safe because we're using Task.FromResult as the only "async" part
		}

		public async Task<TEntity?> GetAsync(TPrimaryKeyEntity entity)
		{
			return await GetInternal(entity, async (pk, query) => await QuerySingleOrDefaultAsync(query, pk));
		}

		private async Task<TEntity?> GetInternal(TPrimaryKeyEntity entity, Func<TPrimaryKeyEntity, string, Task<TEntity?>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			CheckForDefaultPrimaryKeys(info, entity);

			var query = _queryGenerator.GenerateGetQuery();
			var result = await execute(entity, query);

			return result;
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
			var result = InsertInternalAsync(entity, (query, input) => Task.FromResult(QuerySingle(query, input)))
							.GetAwaiter()
							.GetResult();
			InvokePostOperationNotNullable(PostInsert, result);
			return result;
		}

		public async Task<TEntity> InsertAsync(TEntity entity)
		{
			InvokePreOperation(PreInsert, entity);
			var result = await InsertInternalAsync(entity, async (query, input) => await QuerySingleAsync(query, input));
			InvokePostOperationNotNullable(PostInsert, result);
			return result;
		}

		private async Task<TEntity> InsertInternalAsync(TEntity entity, Func<string, TEntity, Task<TEntity>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			var invalidIdentityColumns = info.PrimaryKeys
												.Where(pk => pk.IsIdentity && !pk.HasDefaultValue(entity))
												.ToList();

			if (invalidIdentityColumns.Any())
			{
				throw new ArgumentException($"entity has the following primary keys marked with IsIdentity, which have non-default values: {string.Join(", ", invalidIdentityColumns.Select(col => col.Name))}", nameof(entity));
			}

			var query = _queryGenerator.GenerateInsertQuery(entity);
			return await execute(query, entity);
		}
		#endregion

		#region Update
		public TEntity? Update(TEntity entity)
		{
			InvokePreOperation(PreUpdate, entity);
			var result = UpdateInternalAsync(entity, (input, query) => Task.FromResult(QuerySingleOrDefault(query, input)))
							.GetAwaiter()
							.GetResult();
			InvokePostOperation(PostUpdate, result);
			return result;
		}

		public async Task<TEntity?> UpdateAsync(TEntity entity)
		{
			InvokePreOperation(PreUpdate, entity);
			var result = await UpdateInternalAsync(entity, async (input, query) => await QuerySingleOrDefaultAsync(query, input));
			InvokePostOperation(PostUpdate, result);
			return result;
		}

		private async Task<TEntity?> UpdateInternalAsync(TEntity entity, Func<TEntity, string, Task<TEntity?>> execute)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			CheckForDefaultPrimaryKeys(info, entity);

			var query = _queryGenerator.GenerateUpdateQuery();
			var result = await execute(entity, query);

			return result;
		}
		#endregion

		#region Event invokation
		protected void InvokePreOperation<T>(PreOperationDelegate<T>? @delegate, T entity)
		where T : TPrimaryKeyEntity
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

		private static void CheckForDefaultPrimaryKeys(EntityInformation info, TPrimaryKeyEntity entity)
		{
			var invalidPrimaryKeys = info.PrimaryKeys
											   .Where(pk => pk.HasDefaultValue(entity))
											   .ToList();

			if (invalidPrimaryKeys.Any())
			{
				throw new ArgumentException($"entity has the following primary keys which have default values: {string.Join(", ", invalidPrimaryKeys.Select(col => col.Name))}", nameof(entity));
			}
		}

	}
}
