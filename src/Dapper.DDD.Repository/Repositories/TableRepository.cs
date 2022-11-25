namespace Dapper.DDD.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>,
	ITableRepository<TAggregate, TAggregateId>
	where TAggregate : notnull
	where TAggregateId : notnull
{
	public TableRepository(IOptions<TableAggregateConfiguration<TAggregate>> options,
		IOptions<DefaultConfiguration>? defaultOptions) : base(options.Value, defaultOptions?.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.TableName);
		TableName = options.Value.TableName;
		PropertyList = _queryGenerator.GeneratePropertyList(TableName);
	}

	protected string TableName { get; }

	protected string PropertyList { get; }

	#region ITableRepository

	public virtual async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken = default)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

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

	public virtual async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery(aggregate);
		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken);
	}

	public virtual async Task<TAggregate> UpsertAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpsertQuery(aggregate);
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken);
	}
	#endregion
}