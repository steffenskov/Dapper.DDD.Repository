namespace Dapper.DDD.Repository.QueryGenerators;

public interface IQueryGenerator<in TAggregate>

{
	string GenerateDeleteQuery();

	string GenerateInsertQuery(TAggregate aggregate);

	string GenerateGetAllQuery();

	string GenerateGetQuery();

	string GenerateUpdateQuery(TAggregate aggregate);

	string GeneratePropertyList(string tableName);
	string GenerateUpsertQuery(TAggregate aggregate);
}