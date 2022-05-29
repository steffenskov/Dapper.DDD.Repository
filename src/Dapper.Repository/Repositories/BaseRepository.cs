using Dapper.Repository.Reflection;

namespace Dapper.Repository.Repositories;
public abstract class BaseRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	private readonly IDapperInjectionFactory _dapperInjectionFactory;
	private readonly IDapperInjection<TAggregate> _dapperInjection;
	protected readonly IReadAggregateConfiguration<TAggregate> _configuration;
	protected readonly bool _hasValueObjects;
	private readonly IConnectionFactory _connectionFactory;
	protected readonly IQueryGenerator<TAggregate> _queryGenerator;

	protected BaseRepository(BaseAggregateConfiguration<TAggregate> configuration, DefaultConfiguration? defaultConfiguration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		configuration.SetDefaults(defaultConfiguration);
		ArgumentNullException.ThrowIfNull(configuration.ConnectionFactory);
		ArgumentNullException.ThrowIfNull(configuration.DapperInjectionFactory);
		ArgumentNullException.ThrowIfNull(configuration.QueryGeneratorFactory);

		_dapperInjectionFactory = configuration.DapperInjectionFactory;
		_dapperInjection = _dapperInjectionFactory.Create<TAggregate>();
		_connectionFactory = configuration.ConnectionFactory!;
		_queryGenerator = configuration.QueryGeneratorFactory.Create<TAggregate>(configuration);
		_configuration = configuration;
		_hasValueObjects = _configuration.GetValueObjects().Any();

	}

	/// <summary>
	/// Wraps the id for using it as a param to a query.
	/// </summary>
	protected IDictionary<string, object?> WrapId(TAggregateId id)
	{
		var dictionary = new Dictionary<string, object?>();
		var keys = _configuration.GetKeys();
		if (keys.Count == 1)
		{
			dictionary.Add(keys.First().Name, id);
		}
		else
		{
			foreach (var key in keys)
			{
				dictionary.Add(key.Name, key.GetValue(id));
			}
		}
		return dictionary;
	}

	protected async Task<IEnumerable<TAggregate>> QueryWithValueObjectsAsync(string query, object? param = null)
	{
		var valueObjectProperties = _configuration.GetValueObjects();
		var valueTypes = valueObjectProperties.Select(property => property.Type).ToArray();
		var splitOn = string.Join(",", valueObjectProperties.Select(GetFirstPropertyName));
		return await QueryWithMapAsync(query, valueTypes, Map, param, splitOn: splitOn).ConfigureAwait(false);
	}

	private TAggregate Map(object[] args)
	{
		var result = (TAggregate)args.First(arg => arg is TAggregate);
		foreach (var valueObjectProperty in _configuration.GetValueObjects())
		{
			var valueObject = args.First(arg => arg.GetType() == valueObjectProperty.Type);
			valueObjectProperty.SetValue(result, valueObject);
		}
		return result;
	}

	private string GetFirstPropertyName(ExtendedPropertyInfo property)
	{
		var properties = property.GetPropertiesOrdered();
		return properties.First().Name;
	}

	public async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetAllQuery();
		return await QueryAsync(query, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	#region Dapper methods
	protected async Task<IEnumerable<TAggregate>> QueryWithMapAsync(string query, Type[] types, Func<object[], TAggregate> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QueryAsync(connection, query, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
	}
	protected async Task<IEnumerable<TAggregate>> QueryAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TAggregate?> QuerySingleOrDefaultAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TAggregate> QuerySingleAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<IEnumerable<TResult>> ScalarMultipleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TResult?> ScalarSingleOrDefaultAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<TResult> ScalarSingleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}

	protected async Task<int> ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.ExecuteAsync(connection, query, param, transaction, commandTimeout, commandType, cancellationToken);
	}
	#endregion
}