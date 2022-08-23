namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;

public record CompositeUser
{
	public CompositeUserId Id { get; init; } = default!;
	public DateTime DateCreated { get; private init; } // private init; as I want this value to never be set by the user

	public int? Age { get; init; }
}

public record CompositeUserId(string Username, string Password);