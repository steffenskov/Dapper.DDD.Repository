using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

internal class CustomerWithUser
{
	public int Id { get; set; }
	public UserValueObject User { get; set; } = default!;
}