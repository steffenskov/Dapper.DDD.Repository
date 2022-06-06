using Dapper.Repository.UnitTests.ValueObjects;

namespace Dapper.Repository.UnitTests.Aggregates;

public class AggregateWithValueObjectId
{
	public UserId Id { get; set; } = default!;

	public int Age { get; set; }
}
