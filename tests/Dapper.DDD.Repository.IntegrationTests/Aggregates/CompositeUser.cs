namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;

public record CompositeUser
{
	public CompositeUserId Id { get; init; } = default!;
	public DateTime DateCreated { get; } // no setter as I want this value to never be set by the user

	public int? Age { get; init; }
}

public record CompositeUserId(string Username, string Password);