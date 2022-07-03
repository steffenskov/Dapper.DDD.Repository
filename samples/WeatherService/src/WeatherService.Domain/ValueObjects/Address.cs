namespace WeatherService.Domain.ValueObjects;

public record Address
{
	public string Street { get; init; } = default!;
	public string City { get; init; } = default!;
}
