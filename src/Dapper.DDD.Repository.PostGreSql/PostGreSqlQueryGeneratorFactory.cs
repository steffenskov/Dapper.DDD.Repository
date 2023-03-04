using Dapper.DDD.Repository.QueryGenerators;

namespace Dapper.DDD.Repository.PostGreSql;

public class PostGreSqlQueryGeneratorFactory: IQueryGeneratorFactory
{
	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
		where TAggregate : notnull
	{
		return new PostGreSqlQueryGenerator<TAggregate>(configuration);
	}
}