using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Repository.Interfaces
{
	public interface IViewRepository<TEntity>
	where TEntity : DbEntity
	{
		IEnumerable<TEntity> GetAll();
		Task<IEnumerable<TEntity>> GetAllAsync();
	}
}
