using StrongTypedId;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class StrongUserId : StrongTypedId<StrongUserId, int>
{
	public StrongUserId(int primitiveValue) : base(primitiveValue)
	{
	}
}

public class UserWithStrongTypedId
{
	public StrongUserId? Id { get; set; }
	
	public string Username { get; set; } = default!;
}