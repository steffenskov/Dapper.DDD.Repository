namespace Dapper.Repository.Interfaces;

public interface ITableRepository<TAggregate, TAggregateId>

{
	Task<TAggregate?> DeleteAsync(TAggregateId id, CancellationToken cancellationToken);

	Task<TAggregate?> GetAsync(TAggregateId id, CancellationToken cancellationToken);

	Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken);

	Task<TAggregate> InsertAsync(TAggregate aggregate, CancellationToken cancellationToken);

	Task<TAggregate?> UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken);
}