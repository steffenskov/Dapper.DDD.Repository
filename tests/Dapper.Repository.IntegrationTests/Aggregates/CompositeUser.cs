namespace Dapper.Repository.IntegrationTests.Aggregates;

public record CompositeUser
{
	public CompositeUserId Id { get; init; } = default!;
	public DateTime DateCreated { get; } // No init; as I want this value to never be set by the user

	public int? Age { get; init; }
}

public record CompositeUserId
{
	public string Username { get; init; } = default!;

	public string Password { get; init; } = default!;
}
