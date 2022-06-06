namespace Dapper.Repository.Repositories;

public class TableRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, ITableRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	public TableRepository(IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
	}

	#region ITableRepository
	public async Task<TAggregate?> DeleteAsync(TAggregateId id)
	{
		var query = _queryGenerator.GenerateDeleteQuery();

		return HasValueObjects
				? (await QueryWithValueObjectsAsync(query, WrapId(id))).FirstOrDefault()
				: await QuerySingleOrDefaultAsync(query, WrapId(id));
	}

	public async Task<TAggregate?> GetAsync(TAggregateId id)
	{
		var query = _queryGenerator.GenerateGetQuery();

		return HasValueObjects
				? (await QueryWithValueObjectsAsync(query, WrapId(id))).FirstOrDefault()
				: await QuerySingleOrDefaultAsync(query, WrapId(id));
	}

	public async Task<TAggregate> InsertAsync(TAggregate aggregate)
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
		return HasValueObjects
				? (await QueryWithValueObjectsAsync(query, WrapAggregate(aggregate, false, false))).First()
				: await QuerySingleAsync(query, aggregate);
	}

	public async Task<TAggregate?> UpdateAsync(TAggregate aggregate)
	{
		ArgumentNullException.ThrowIfNull(aggregate);
		var query = _queryGenerator.GenerateUpdateQuery();
		return HasValueObjects
				? (await QueryWithValueObjectsAsync(query, WrapAggregate(aggregate, true, true))).FirstOrDefault()
				: await QuerySingleOrDefaultAsync(query, aggregate);
	}
	#endregion
}
