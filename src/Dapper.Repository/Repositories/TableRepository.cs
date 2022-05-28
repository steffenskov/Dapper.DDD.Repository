using Dapper.Repository.Reflection;

namespace Dapper.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : ITableRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	private readonly IDapperInjectionFactory _dapperInjectionFactory;
	private readonly IDapperInjection<TAggregate> _dapperInjection;
	private readonly IReadAggregateConfiguration<TAggregate> _configuration;
	private readonly bool _hasValueObjects;
	private readonly IConnectionFactory _connectionFactory;
	private readonly IQueryGenerator<TAggregate> _queryGenerator;

	public TableRepository(IOptions<AggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions)
	{
		ArgumentNullException.ThrowIfNull(options);
		ArgumentNullException.ThrowIfNull(options.Value);

		var configuration = options.Value;
		configuration.SetDefaults(defaultOptions?.Value);
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

	#region ITableRepository
	public async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetQuery();

		if (_hasValueObjects)
		{
			return (await QueryWithValueObjectsAsync(query, id).ConfigureAwait(false)).FirstOrDefault();
		}
		else
			return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	private async Task<IEnumerable<TAggregate>> QueryWithValueObjectsAsync(string query, TAggregateId id)
	{
		var valueObjectProperties = _configuration.GetValueObjects();
		var valueTypes = valueObjectProperties.Select(property => property.Type).ToArray();
		var splitOn = string.Join(",", valueObjectProperties.Select(GetFirstPropertyName));
		return await QueryWithMapAsync(query, valueTypes, Map, WrapId(id), splitOn: splitOn).ConfigureAwait(false);
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
		var properties = TypePropertiesCache.GetProperties(property.Type);
		var firstProperty = properties.Values.OrderBy(prop => prop.Name).First().Name;
		return $"{property.Name}_{firstProperty}"; // TODO: This kind of formatting is used both here and in the QueryGenerator - abstract away from here
	}

	public async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken)
	{
		var query = _queryGenerator.GenerateGetAllQuery();
		return await QueryAsync(query, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var invalidIdentityProperties = _configuration.GetIdentityProperties()
											.Where(pk => !pk.HasDefaultValue(aggregate))
											.ToList();

		if (invalidIdentityProperties.Any())
		{
			throw new ArgumentException($"Aggregate has the following identity properties, which have non-default values: {string.Join(", ", invalidIdentityProperties.Select(col => col.Name))}", nameof(aggregate));
		}
		var query = _queryGenerator.GenerateInsertQuery(aggregate);
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery();
		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken).ConfigureAwait(false);
	}
	#endregion

	private IDictionary<string, object?> WrapId(TAggregateId id)
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
