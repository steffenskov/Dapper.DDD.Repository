using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Repository
{
	public delegate T QuerySingleDelegate<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

	public delegate IEnumerable<T> QueryDelegate<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

	public delegate Task<IEnumerable<T>> QueryAsyncDelegate<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

	public delegate Task<T> QuerySingleAsyncDelegate<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

	public delegate int ExecuteDelegate(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

	public delegate Task<int> ExecuteAsyncDelegate(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

	public interface IDapperInjection<TEntity>
	{
		QuerySingleDelegate<TEntity> QuerySingle { get; }

		QuerySingleDelegate<TEntity> QuerySingleOrDefault { get; }

		QueryDelegate<TEntity> Query { get; }

		QuerySingleAsyncDelegate<TEntity> QuerySingleAsync { get; }

		QuerySingleAsyncDelegate<TEntity> QuerySingleOrDefaultAsync { get; }

		QueryAsyncDelegate<TEntity> QueryAsync { get; }

		ExecuteDelegate Execute { get; }

		ExecuteAsyncDelegate ExecuteAsync { get; }
	}
}