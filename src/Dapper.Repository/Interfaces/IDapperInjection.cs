namespace Dapper.Repository.Interfaces;

public interface IDapperInjection<T>
{
	Task<T> QuerySingleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
	Task<T> QuerySingleOrDefaultAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
	Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
	Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
	Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, Type[] types, Func<object[], T> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
}