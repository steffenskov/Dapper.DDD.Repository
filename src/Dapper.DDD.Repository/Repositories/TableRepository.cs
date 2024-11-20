namespace Dapper.DDD.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>,
	ITableRepository<TAggregate, TAggregateId>
	where TAggregate : notnull
	where TAggregateId : notnull
{
	private readonly bool _hasTriggers;

	public TableRepository(IOptions<TableAggregateConfiguration<TAggregate>> options,
		IOptions<DefaultConfiguration>? defaultOptions) : base(options.Value, defaultOptions?.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.TableName);
		_hasTriggers = options.Value.HasTriggers;
		TableName = options.Value.TableName;
		PropertyList = _queryGenerator.GeneratePropertyList(TableName);
	}

	protected string TableName { get; }

	protected string PropertyList { get; }

	#region ITableRepository

	/// <summary>
	///     Executes a delete in the database and returns the deleted aggregate read from DB.
	///     NOTE: If Triggers are enabled in the table configuration, this will return the null and not attempt to read from
	///     DB!
	/// </summary>
	public virtual async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken = default)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		if (_hasTriggers)
		{
			await ExecuteAsync(query, WrapId(id), cancellationToken: cancellationToken);
			return default;
		}

		return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken);
	}

	public virtual async Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var invalidIdentityProperties = _configuration.GetIdentityProperties()
			.Where(pk => !pk.HasDefaultValue(aggregate))
			.ToList();

		if (invalidIdentityProperties.Any())
		{
			throw new ArgumentException(
				$"Aggregate has the following identity properties, which have non-default values: {string.Join(", ", invalidIdentityProperties.Select(col => col.Name))}",
				nameof(aggregate));
		}

		var query = _queryGenerator.GenerateInsertQuery(aggregate);
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken);
	}

	/// <summary>
	///     Executes an update in the database and returns the updated aggregate read from DB.
	///     NOTE: If Triggers are enabled in the table configuration, this will return the aggregate argument to the method and
	///     not attempt to read from DB!
	/// </summary>
	public virtual async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery(aggregate);
		if (_hasTriggers)
		{
			var rowCount = await ExecuteAsync(query, aggregate, cancellationToken: cancellationToken);
			return rowCount > 0 ? aggregate : default;
		}

		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken);
	}

	public virtual async Task<TAggregate> UpsertAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		if (_hasTriggers)
		{
			throw new InvalidOperationException("Upsert is not supported on tables with triggers");
		}

		var query = _queryGenerator.GenerateUpsertQuery(aggregate);

		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken);
	}

	#endregion
}