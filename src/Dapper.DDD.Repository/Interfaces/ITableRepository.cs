namespace Dapper.DDD.Repository.Interfaces;

public interface ITableRepository<TAggregate, TAggregateId>
{
	Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken = default);
	Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken = default);
	Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
	Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
}
