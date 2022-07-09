namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;

public record Customer
{
	public Guid Id { get; init; }
	public string Name { get; init; } = default!;
	public Address InvoiceAddress { get; init; } = default!;
	public Address DeliveryAddress { get; init; } = default!;
}

public record Address
{
	public string Street { get; init; } = default!;
	public int Zipcode { get; init; }
}
