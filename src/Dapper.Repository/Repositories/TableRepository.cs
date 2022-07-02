namespace Dapper.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, ITableRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	protected string TableName { get; }

	public TableRepository(IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.TableName);
		TableName = options.Value.TableName;
	}

	#region ITableRepository
	public async Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken = default)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		return await QuerySingleOrDefaultAsync(query, WrapId(id), cancellationToken: cancellationToken);
	}

	public async Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
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
		return await QuerySingleAsync(query, aggregate, cancellationToken: cancellationToken);
	}

	public async Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery(aggregate);
		return await QuerySingleOrDefaultAsync(query, aggregate, cancellationToken: cancellationToken);
	}
	#endregion
}
