using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class AggregateWithWrappedPrimitive
{
	public WrappedPrimitive Wrapped { get; init; }
	public WrappedPrimitive? NullableWrapped { get; init; }
}