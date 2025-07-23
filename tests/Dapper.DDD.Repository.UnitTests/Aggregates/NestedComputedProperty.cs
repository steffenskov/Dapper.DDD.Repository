namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class NestedComputedPropertyAggregate
{
	public ValueObjectWithComputedProperty Value { get; set; } = default!;
}

public record ValueObjectWithComputedProperty(int Value)
{
	public string Description => Value.ToString();
}