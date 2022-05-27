using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.IntegrationTests;

public class DapperInjection<T> : IDapperInjection<T>
{
	public Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T> QuerySingleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T> QuerySingleOrDefaultAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}
}
