using Dapper.Repository.UnitTests.ValueObjects;

namespace Dapper.Repository.UnitTests.Aggregates;
public record UserAggregate
{
	public Guid Id { get; init; }
	public Address DeliveryAddress { get; init; } = default!;
	public Address InvoiceAddress { get; init; } = default!;
}
