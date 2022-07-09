using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.ValueObjects;

public class UserValueObject
{
	public StrongUserId Id { get; set; } = default!;
	public string Username { get; set; } = default!;
}
