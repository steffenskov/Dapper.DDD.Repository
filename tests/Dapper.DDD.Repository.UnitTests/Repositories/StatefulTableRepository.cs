using Dapper.DDD.Repository.Repositories;

namespace Dapper.DDD.Repository.UnitTests.Repositories;

public class StatefulAggregate
{
	public Guid Id { get; set; }
}

public interface IStatefulTableRepository
{
	Guid State { get; }
}

public class StatefulTableRepository : TableRepository<StatefulAggregate, Guid>, IStatefulTableRepository
{
	public Guid State { get; init; }
	
	public StatefulTableRepository(IOptions<TableAggregateConfiguration<StatefulAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}

public interface IStatefulViewRepository
{
	Guid State { get; }
}

public class StatefulViewRepository : ViewRepository<StatefulAggregate, Guid>, IStatefulViewRepository
{
	public Guid State { get; init; }

	public StatefulViewRepository(IOptions<ViewAggregateConfiguration<StatefulAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}

public interface IStatefulSimpleViewRepository
{
	Guid State { get; }
}

public class StatefulSimpleViewRepository : ViewRepository<StatefulAggregate>, IStatefulSimpleViewRepository
{
	public Guid State { get; init; }

	public StatefulSimpleViewRepository(IOptions<ViewAggregateConfiguration<StatefulAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}