namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public record HasDefaultConstraintAggregate
{
	public int Id { get; init; }

	public DateTime DateCreated { get; init; }
}