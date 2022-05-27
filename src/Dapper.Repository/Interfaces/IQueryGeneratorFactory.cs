namespace Dapper.Repository.Interfaces;
public interface IQueryGeneratorFactory
{
	IQueryGenerator<TAggregate> Create<TAggregate>(AggregateConfiguration<TAggregate> configuration) where TAggregate : notnull;
}