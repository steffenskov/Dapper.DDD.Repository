namespace Dapper.Repository.Interfaces;

public interface ITableRepository<TAggregate, TAggregateId>
{
	Task<TAggregate?> DeleteAsync(TAggregateId id);
	Task<TAggregate?> GetAsync(TAggregateId id);
	Task<IEnumerable<TAggregate>> GetAllAsync();
	Task<TAggregate> InsertAsync(TAggregate aggregate);
	Task<TAggregate?> UpdateAsync(TAggregate aggregate);
}