namespace Dapper.DDD.Repository.Sql.IntegrationTests.Models;

public record TriggerEntityWithoutIdentity
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public DateTime DateCreated { get; init; }
}