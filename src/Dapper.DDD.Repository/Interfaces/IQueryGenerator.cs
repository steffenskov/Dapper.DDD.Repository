namespace Dapper.DDD.Repository.Interfaces;

public interface IQueryGenerator<TAggregate>

{
	string GenerateDeleteQuery();

	string GenerateInsertQuery(TAggregate aggregate);

	string GenerateGetAllQuery();

	string GenerateGetQuery();

	string GenerateUpdateQuery(TAggregate aggregate);
}
