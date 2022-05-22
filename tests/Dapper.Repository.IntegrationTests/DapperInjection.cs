namespace Dapper.Repository.IntegrationTests
{
	internal class DapperInjection<TEntity> : IDapperInjection<TEntity>
	{
		public QuerySingleDelegate<TEntity> QuerySingle => SqlMapper.QuerySingle<TEntity>;

		public QuerySingleDelegate<TEntity> QuerySingleOrDefault => SqlMapper.QuerySingleOrDefault<TEntity>;

		public QueryDelegate<TEntity> Query => SqlMapper.Query<TEntity>;

		public QuerySingleAsyncDelegate<TEntity> QuerySingleAsync => SqlMapper.QuerySingleAsync<TEntity>;

		public QuerySingleAsyncDelegate<TEntity> QuerySingleOrDefaultAsync => SqlMapper.QuerySingleOrDefaultAsync<TEntity>;

		public QueryAsyncDelegate<TEntity> QueryAsync => SqlMapper.QueryAsync<TEntity>;

		public ExecuteDelegate Execute => SqlMapper.Execute;

		public ExecuteAsyncDelegate ExecuteAsync => SqlMapper.ExecuteAsync;
	}
}