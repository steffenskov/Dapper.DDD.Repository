namespace Dapper.Repository.Repositories;
public class ViewRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>, IViewRepository<TAggregate, TAggregateId>
where TAggregate : notnull
where TAggregateId : notnull
{
	protected string ViewName { get; }

	public ViewRepository(IOptions<ViewAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.ViewName);
		ViewName = options.Value.ViewName;
	}
}

public class ViewRepository<TAggregate> : BaseRepository<TAggregate>, IViewRepository<TAggregate>
where TAggregate : notnull
{
	protected string ViewName { get; }

	public ViewRepository(IOptions<ViewAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options.Value, defaultOptions.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.ViewName);
		ViewName = options.Value.ViewName;
	}
}
