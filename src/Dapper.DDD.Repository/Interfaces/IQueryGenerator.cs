namespace Dapper.DDD.Repository.Interfaces;

public interface IQueryGenerator<in TAggregate>

{
	string GenerateDeleteQuery();

	string GenerateInsertQuery(TAggregate aggregate);

	string GenerateGetAllQuery();

	string GenerateGetQuery();

	string GenerateUpdateQuery(TAggregate aggregate);

	string GeneratePropertyList(string tableName);
}
