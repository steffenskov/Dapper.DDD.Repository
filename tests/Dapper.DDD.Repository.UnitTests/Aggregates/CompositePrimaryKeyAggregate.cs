namespace Dapper.DDD.Repository.UnitTests.Aggregates;

internal record CompositePrimaryKeyAggregate
{
	public string Username { get; init; } = default!;
	public string Password { get; init; } = default!;

	public DateTime DateCreated { get; init; }
}