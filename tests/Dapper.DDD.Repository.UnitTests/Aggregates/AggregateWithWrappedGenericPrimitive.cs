using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class AggregateWithWrappedGenericPrimitive
{
	public WrappedGenericPrimitive<double> Double { get; init; }
	public WrappedGenericPrimitive<Guid>? Guid { get; init; }
}