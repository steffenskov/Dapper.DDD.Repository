using WeatherService.Model.ValueObjects;

namespace WeatherService.Model.Entities;

public class WeatherStation
{
	public int Id { get; private init; } // init because it's an identity field in the database, as such the code should never set it explicitly
	public string Name { get; set; } = default!;
	public Address Address { get; set; } = default!;
}
