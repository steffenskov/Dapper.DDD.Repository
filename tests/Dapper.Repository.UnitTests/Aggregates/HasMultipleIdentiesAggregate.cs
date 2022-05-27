namespace Dapper.Repository.UnitTests.Aggregates;
public record HasMultipleIdentiesAggregate
{
	public int Id { get; init; }
	public int Counter { get; init; }
	public string Name { get; init; } = default!;
}