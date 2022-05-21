using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Repository.Interfaces
{
	public interface IHeapRepository<TEntity>
	where TEntity : DbEntity
	{
		TEntity? Delete(TEntity entity);
		Task<TEntity?> DeleteAsync(TEntity entity);

		TEntity? Get(TEntity entity);
		Task<TEntity?> GetAsync(TEntity entity);

		IEnumerable<TEntity> GetAll();
		Task<IEnumerable<TEntity>> GetAllAsync();

		TEntity Insert(TEntity entity);
		Task<TEntity> InsertAsync(TEntity entity);

	}
}
