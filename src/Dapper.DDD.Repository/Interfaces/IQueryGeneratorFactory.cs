namespace Dapper.DDD.Repository.Interfaces;
public interface IQueryGeneratorFactory
{
	IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration) where TAggregate : notnull;
}