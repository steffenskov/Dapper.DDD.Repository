namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public record AllPropertiesHasMissingSetterAggregate
{
	public int Id { get; init; }

	public DateTime DateCreated { get; }
}