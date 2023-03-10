namespace Dapper.DDD.Repository.QueryGenerators;

public interface IQueryGeneratorFactory
{
	IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
		where TAggregate : notnull;
}