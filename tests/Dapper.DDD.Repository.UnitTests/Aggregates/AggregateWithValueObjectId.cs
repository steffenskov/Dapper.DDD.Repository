using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class AggregateWithValueObjectId
{
	public UserId Id { get; set; } = default!;

	public int Age { get; set; }
}
