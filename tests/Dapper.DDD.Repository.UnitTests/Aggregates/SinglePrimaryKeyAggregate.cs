namespace Dapper.DDD.Repository.UnitTests.Aggregates
{
	public record SinglePrimaryKeyAggregate
	{
		public int Id { get; init; }

		public string Username { get; init; } = default!;

		public string Password { get; init; } = default!;
	}
}
