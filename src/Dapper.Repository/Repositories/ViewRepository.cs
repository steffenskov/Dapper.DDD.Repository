namespace Dapper.Repository.Repositories;
public class ViewRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, IViewRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	public ViewRepository(IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
	}
}