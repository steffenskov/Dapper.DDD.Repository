using StrongTypedId;

namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;

public class Zipcode : StrongTypedId<Zipcode, int>
{
	public Zipcode(int primitiveValue) : base(primitiveValue)
	{
	}
}

public record Customer
{
	public Guid Id { get; init; }
	public string Name { get; init; } = default!;
	public Address InvoiceAddress { get; init; } = default!;
	public Address DeliveryAddress { get; init; } = default!;
	
	public string IdAndName =>
		$"{Name} ({Id})"; // Only here to test .Ignore works correctly when configuring aggregates
}

public record Address(string Street, Zipcode Zipcode);

public record CustomerWithNestedAddresses
{
	public Guid Id { get; init; }
	public string Name { get; init; } = default!;
	public Addresses Addresses { get; init; } = default!;
}

public record Addresses(Address Invoice, Address Delivery);