using StrongTypedId;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class StrongUserId : StrongTypedId<StrongUserId, int>
{
	public StrongUserId(int primitiveId) : base(primitiveId)
	{
	}
}

public class UserWithStrongTypedId
{
	public StrongUserId Id { get; set; } = default!;

	public string Username { get; set; } = default!;

}
