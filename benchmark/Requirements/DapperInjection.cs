using System.Data;
using Dapper;
using Dapper.DDD.Repository.Interfaces;

namespace benchmark.Requirements;

public class DapperInjection<T> : IDapperInjection<T>
where T : notnull
{
	public Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QueryAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T> QuerySingleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<object> QuerySingleAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T?> QuerySingleOrDefaultAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleOrDefaultAsync<T?>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleOrDefaultAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}
}
