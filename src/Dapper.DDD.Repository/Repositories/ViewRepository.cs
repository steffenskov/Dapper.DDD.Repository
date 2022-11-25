namespace Dapper.DDD.Repository.Repositories;

public class ViewRepository<TAggregate, TAggregateId> : BaseRepository<TAggregate, TAggregateId>,
	IViewRepository<TAggregate, TAggregateId>
	where TAggregate : notnull
	where TAggregateId : notnull
{
	public ViewRepository(IOptions<ViewAggregateConfiguration<TAggregate>> options,
		IOptions<DefaultConfiguration>? defaultOptions) : base(options.Value, defaultOptions?.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.ViewName);
		ViewName = options.Value.ViewName;
		PropertyList = _queryGenerator.GeneratePropertyList(ViewName);
	}

	protected string ViewName { get; }

	protected string PropertyList { get; }
}

public class ViewRepository<TAggregate> : BaseRepository<TAggregate>, IViewRepository<TAggregate>
	where TAggregate : notnull
{
	public ViewRepository(IOptions<ViewAggregateConfiguration<TAggregate>> options,
		IOptions<DefaultConfiguration>? defaultOptions) : base(options.Value, defaultOptions?.Value)
	{
		ArgumentNullException.ThrowIfNull(options.Value.ViewName);
		ViewName = options.Value.ViewName;
		PropertyList = _queryGenerator.GeneratePropertyList(ViewName);
	}

	protected string ViewName { get; }
	protected string PropertyList { get; }
}