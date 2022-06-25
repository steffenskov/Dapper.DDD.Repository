namespace benchmark.Aggregates;

public class Customer
{
	public Guid Id { get; init; }
	public string Name { get; init; } = default!;
	public Address InvoiceAddress { get; init; } = default!;
	public Address DeliveryAddress { get; init; } = default!;
}

public class Address
{
	public string Street { get; init; } = default!;
	public int Zipcode { get; init; }
}
