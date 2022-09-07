namespace Dapper.DDD.Repository.Sql;

public class SqlQueryGeneratorFactory : IQueryGeneratorFactory
{
	private readonly IList<Predicate<Type>> _serializeColumnTypePredicates;

	public SqlQueryGeneratorFactory()
	{
		_serializeColumnTypePredicates = new List<Predicate<Type>>();
	}

	public IQueryGenerator<TAggregate> Create<TAggregate>(BaseAggregateConfiguration<TAggregate> configuration)
		where TAggregate : notnull
	{
		return configuration is not BaseAggregateConfiguration<TAggregate> sqlConfiguration
			? throw new ArgumentException(
				$"Configuration must be of type {nameof(BaseAggregateConfiguration<TAggregate>)}")
			: (IQueryGenerator<TAggregate>)new SqlQueryGenerator<TAggregate>(sqlConfiguration,
				_serializeColumnTypePredicates);
	}

	public SqlQueryGeneratorFactory SerializeColumnType(Predicate<Type> typePredicate)
	{
		_serializeColumnTypePredicates.Add(typePredicate);
		return this;
	}
}