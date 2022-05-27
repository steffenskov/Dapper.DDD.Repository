namespace Dapper.Repository.Interfaces;

public interface IViewRepository<TAggregate>

{
	Task<IEnumerable<TAggregate>> GetAllAsync();
}
