namespace Dapper.Repository.Repositories;
public class ViewRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, IViewRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{

	protected string ViewName { get; }

	public ViewRepository(IOptions<ViewAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
		ViewName = options.Value.ViewName;
	}
}
