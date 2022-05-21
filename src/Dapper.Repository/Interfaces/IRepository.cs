using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Repository.Interfaces
{
	public interface IRepository<TPrimaryKeyEntity, TEntity>
	where TPrimaryKeyEntity : DbEntity
	where TEntity : TPrimaryKeyEntity
	{
		TEntity? Delete(TPrimaryKeyEntity entity);
		Task<TEntity?> DeleteAsync(TPrimaryKeyEntity entity);

		TEntity? Get(TPrimaryKeyEntity entity);
		Task<TEntity?> GetAsync(TPrimaryKeyEntity entity);

		IEnumerable<TEntity> GetAll();
		Task<IEnumerable<TEntity>> GetAllAsync();

		TEntity Insert(TEntity entity);
		Task<TEntity> InsertAsync(TEntity entity);

		TEntity? Update(TEntity entity);
		Task<TEntity?> UpdateAsync(TEntity entity);
	}
}
