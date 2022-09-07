namespace Dapper.DDD.Repository.Interfaces;

public interface IViewRepository<TAggregate, TAggregateId> : IViewRepository<TAggregate>
{
	Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken = default);
}

public interface IViewRepository<TAggregate>
{
	Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
}