namespace Dapper.Repository.Interfaces;

public interface IViewRepository<TAggregate, TAggregateId>
{
	Task<IEnumerable<TAggregate>> GetAllAsync();
}
