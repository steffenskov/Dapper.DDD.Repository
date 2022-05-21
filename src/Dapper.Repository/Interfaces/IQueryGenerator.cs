namespace Dapper.Repository.Interfaces
{
	public interface IQueryGenerator<TEntity>
	where TEntity : DbEntity
	{
		string GenerateDeleteQuery();

		string GenerateInsertQuery(TEntity entity);

		string GenerateGetAllQuery();

		string GenerateGetQuery();

		string GenerateUpdateQuery();
	}
}
