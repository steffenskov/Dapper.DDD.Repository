namespace benchmark.Aggregates;
public record CustomerFlat
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public string InvoiceAddress_Street { get; set; } = default!;
	public int InvoiceAddress_Zipcode { get; set; }
	public string DeliveryAddress_Street { get; set; } = default!;
	public int DeliveryAddress_Zipcode { get; set; }
}
