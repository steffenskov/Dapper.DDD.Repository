using Dapper.DDD.Repository.Exceptions;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Repositories;

public abstract class BaseRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate>
where TAggregate : notnull
where TAggregateId : notnull
{
	protected BaseRepository(BaseAggregateConfiguration<TAggregate> configuration, DefaultConfiguration? defaultConfiguration) : base(configuration, defaultConfiguration)
	{
		var readConfig = (IReadAggregateConfiguration<TAggregate>)configuration;
		if (readConfig.GetKeys().Count == 0)
		{
			throw new ArgumentException("No key has been specified for this aggregate.", nameof(configuration));
		}
	}

	public async Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken = default)
	{
		var query = _queryGenerator.GenerateGetQuery();

		return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken);
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

	private void AddWrappedValue(IDictionary<string, object?> dictionary, ExtendedPropertyInfo property, object? value)
	{
		if (value is null)
		{
			dictionary[property.Name] = value;
		}
		else if (property.Type.IsSimpleOrBuiltIn())
		{
			dictionary[property.Name] = value;
		}
		else if (_objectFlattener.TryGetTypeConverter(property.Type, out var converter))
		{
			dictionary[property.Name] = converter!.ConvertToSimple(value);
		}
		else
		{
			foreach (var propertyInfo in property.GetPropertiesOrdered())
			{
				dictionary.Add(propertyInfo.Name, value is not null ? propertyInfo.GetValue(value) : null);
			}
		}
	}
}

public abstract class BaseRepository<TAggregate>
where TAggregate : notnull
{
	private readonly IDapperInjectionFactory _dapperInjectionFactory;
	private readonly IDapperInjection<TAggregate> _dapperInjection;
	private protected readonly IReadAggregateConfiguration<TAggregate> _configuration;
	private readonly IConnectionFactory _connectionFactory;
	private protected readonly IQueryGenerator<TAggregate> _queryGenerator;
	private protected readonly ObjectFlattener _objectFlattener;
	private readonly bool _shouldFlattenAggregate;

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

		ObjectFlattener.SetTypeProperties(typeof(TAggregate), ((IReadAggregateConfiguration<TAggregate>)configuration).GetProperties());

		_objectFlattener = new ObjectFlattener();
		if (defaultConfiguration is not null)
		{
			foreach (var typeConverter in defaultConfiguration._typeConverters)
			{
				_objectFlattener.AddTypeConverter(typeConverter.Key, typeConverter.Value);
			}
		}

		_shouldFlattenAggregate = _objectFlattener.ShouldFlattenType<TAggregate>(); // Call this last to ensure all type converters are in place first
	}

	public async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var query = _queryGenerator.GenerateGetAllQuery();
		return await QueryAsync(query, cancellationToken: cancellationToken);
	}

	#region Dapper methods
	protected async Task<IEnumerable<TAggregate>> QueryAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			if (_shouldFlattenAggregate)
			{
				var flatType = _objectFlattener.GetFlattenedType<TAggregate>();
				var result = await _dapperInjection.QueryAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result.Select(_objectFlattener.Unflatten<TAggregate>);
			}
			else
			{
				return await _dapperInjection.QueryAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<TAggregate?> QuerySingleOrDefaultAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			if (_shouldFlattenAggregate)
			{
				var flatType = _objectFlattener.GetFlattenedType<TAggregate>();
				var result = await _dapperInjection.QuerySingleOrDefaultAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result is not null ? _objectFlattener.Unflatten<TAggregate>(result) : default;
			}
			else
			{
				return await _dapperInjection.QuerySingleOrDefaultAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<TAggregate> QuerySingleAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			if (_shouldFlattenAggregate)
			{
				var flatType = _objectFlattener.GetFlattenedType<TAggregate>();
				var result = await _dapperInjection.QuerySingleAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return _objectFlattener.Unflatten<TAggregate>(result);
			}
			else
			{
				return await _dapperInjection.QuerySingleAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<IEnumerable<TResult>> ScalarMultipleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			if (_objectFlattener.TryGetTypeConverter(typeof(TResult), out var converter))
			{
				var simpleType = converter!.SimpleType;
				var result = await _dapperInjection.QueryAsync(connection, simpleType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result.Select(simple => (TResult)converter.ConvertToComplex(simple));
			}
			else if (_objectFlattener.ShouldFlattenType<TResult>())
			{
				var flatType = _objectFlattener.GetFlattenedType<TResult>();
				var result = await _dapperInjection.QueryAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result.Select(_objectFlattener.Unflatten<TResult>);
			}
			else
			{
				var dapperInjection = _dapperInjectionFactory.Create<TResult>();
				return await dapperInjection.QueryAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<TResult?> ScalarSingleOrDefaultAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			if (_objectFlattener.TryGetTypeConverter(typeof(TResult), out var converter))
			{
				var simpleType = converter!.SimpleType;
				var result = await _dapperInjection.QuerySingleOrDefaultAsync(connection, simpleType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result is not null ? (TResult)converter.ConvertToComplex(result) : default;
			}
			else if (_objectFlattener.ShouldFlattenType<TResult>())
			{
				var flatType = _objectFlattener.GetFlattenedType<TResult>();
				var result = await _dapperInjection.QuerySingleOrDefaultAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return result is not null ? _objectFlattener.Unflatten<TResult>(result) : default;
			}
			else
			{
				var dapperInjection = _dapperInjectionFactory.Create<TResult>();
				return await dapperInjection.QuerySingleOrDefaultAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<TResult> ScalarSingleAsync<TResult>(string query, object? param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	where TResult : notnull
	{
		using var connection = _connectionFactory.CreateConnection();

		try
		{
			if (_objectFlattener.TryGetTypeConverter(typeof(TResult), out var converter))
			{
				var simpleType = converter!.SimpleType;
				var result = await _dapperInjection.QuerySingleAsync(connection, simpleType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return (TResult)converter.ConvertToComplex(result);
			}
			else if (_objectFlattener.ShouldFlattenType<TResult>())
			{
				var flatType = _objectFlattener.GetFlattenedType<TResult>();
				var result = await _dapperInjection.QuerySingleAsync(connection, flatType, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
				return _objectFlattener.Unflatten<TResult>(result);
			}
			else
			{
				var dapperInjection = _dapperInjectionFactory.Create<TResult>();
				return await dapperInjection.QuerySingleAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
			}
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}

	protected async Task<int> ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		using var connection = _connectionFactory.CreateConnection();
		try
		{
			return await _dapperInjection.ExecuteAsync(connection, query, _objectFlattener.Flatten(param), transaction, commandTimeout, commandType, cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DapperRepositoryQueryException(query, ex);
		}
	}
	#endregion
}
