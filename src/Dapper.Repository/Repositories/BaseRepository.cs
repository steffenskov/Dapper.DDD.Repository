using Dapper.Repository.Reflection;

namespace Dapper.Repository.Repositories;
public abstract class BaseRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	private readonly IDapperInjectionFactory _dapperInjectionFactory;
	private readonly IDapperInjection<TAggregate> _dapperInjection;
	private protected readonly IReadAggregateConfiguration<TAggregate> _configuration;
	private bool HasValueObjects => _valueObjects.Count > 0;
	private readonly IReadOnlyList<ExtendedPropertyInfo> _valueObjects;
	private readonly IConnectionFactory _connectionFactory;
	private protected readonly IQueryGenerator<TAggregate> _queryGenerator;
	protected string EntityName { get; }

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
		_queryGenerator = configuration.QueryGeneratorFactory.Create(configuration);
		_configuration = configuration;
		_valueObjects = _configuration.GetValueObjects();
		EntityName = configuration.EntityName;
	}

	public async Task<IEnumerable<TAggregate>> GetAllAsync()
	{
		var query = _queryGenerator.GenerateGetAllQuery();
		return await QueryAsync(query);
	}

	/// <summary>
	/// Wraps the id for using it as a param to a query.
	/// </summary>
	private protected IDictionary<string, object?> WrapId(TAggregateId id)
	{
		var dictionary = new Dictionary<string, object?>();
		var keys = _configuration.GetKeys();
		if (keys.Count == 1)
		{
			AddWrappedValue(dictionary, keys[0], id);
		}
		else
		{
			foreach (var key in keys)
			{
				AddWrappedValue(dictionary, key, key.GetValue(id));
			}
		}
		return dictionary;
	}

	/// <summary>
	/// Wraps the aggregate for using it as a param to a query if it holds any value objects.
	/// </summary>
	private protected object WrapAggregateIfNecessary(TAggregate aggregate, bool includeIdentities, bool includeDefaults)
	{
		if (!HasValueObjects)
		{
			return aggregate;
		}

		var dictionary = new Dictionary<string, object?>();
		var properties = _configuration.GetProperties();
		var identities = _configuration.GetIdentityProperties();
		var defaults = _configuration.GetPropertiesWithDefaultConstraints();
		foreach (var property in properties)
		{
			if (!includeIdentities && identities.Contains(property))
			{
				continue;
			}

			if (!includeDefaults && defaults.Contains(property))
			{
				continue;
			}

			AddWrappedValue(dictionary, property, property.GetValue(aggregate));
		}
		return dictionary;
	}

	private void AddWrappedValue(IDictionary<string, object?> dictionary, ExtendedPropertyInfo property, object? value)
	{
		if (_valueObjects.Contains(property))
		{
			foreach (var propertyInfo in property.GetPropertiesOrdered())
			{
				dictionary.Add(propertyInfo.Name, value is not null ? propertyInfo.GetValue(value) : null);
			}
		}
		else
		{
			dictionary[property.Name] = value;
		}
	}

	private async Task<IEnumerable<TAggregate>> QueryWithValueObjectsAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		var allTypes = new[] { ObjectFlattener.GetFlattenedType<TAggregate>() };
		return await QueryWithMapAsync(query, allTypes, Map, param, transaction, buffered: true, splitOn: string.Empty, commandTimeout, commandType);
	}

	private TAggregate Map(object[] args)
	{
		return ObjectFlattener.Unflatten<TAggregate>(args[0]);
	}

	private string GetFirstPropertyName(ExtendedPropertyInfo property)
	{
		var properties = property.GetPropertiesOrdered();
		return properties.First().Name;
	}

	#region Dapper methods
	protected async Task<IEnumerable<TAggregate>> QueryWithMapAsync(string query, Type[] types, Func<object[], TAggregate> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.QueryAsync(connection, query, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
	}
	protected async Task<IEnumerable<TAggregate>> QueryAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		if (HasValueObjects)
		{
			return await QueryWithValueObjectsAsync(query, param, transaction, commandTimeout, commandType);
		}
		else
		{
			using var connection = _connectionFactory.CreateConnection();
			return await _dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType);
		}
	}

	protected async Task<TAggregate?> QuerySingleOrDefaultAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		if (HasValueObjects)
		{
			return (await QueryWithValueObjectsAsync(query, param, transaction, commandTimeout, commandType)).FirstOrDefault();
		}
		else
		{
			using var connection = _connectionFactory.CreateConnection();
			return await _dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType);
		}
	}

	protected async Task<TAggregate> QuerySingleAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		if (HasValueObjects)
		{
			return (await QueryWithValueObjectsAsync(query, param, transaction, commandTimeout, commandType)).Single();
		}
		else
		{
			using var connection = _connectionFactory.CreateConnection();
			return await _dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType);
		}
	}

	protected async Task<IEnumerable<TResult>> ScalarMultipleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QueryAsync(connection, query, param, transaction, commandTimeout, commandType);
	}

	protected async Task<TResult?> ScalarSingleOrDefaultAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleOrDefaultAsync(connection, query, param, transaction, commandTimeout, commandType);
	}

	protected async Task<TResult> ScalarSingleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();
		var dapperInjection = _dapperInjectionFactory.Create<TResult>();
		return await dapperInjection.QuerySingleAsync(connection, query, param, transaction, commandTimeout, commandType);
	}

	protected async Task<int> ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		using var connection = _connectionFactory.CreateConnection();
		return await _dapperInjection.ExecuteAsync(connection, query, param, transaction, commandTimeout, commandType);
	}
	#endregion
}
