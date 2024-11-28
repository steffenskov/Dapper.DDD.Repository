namespace Dapper.DDD.Repository.Sql.IntegrationTests.Models;

public record TriggerEntityWithIdentity
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public DateTime DateCreated { get; init; }
}