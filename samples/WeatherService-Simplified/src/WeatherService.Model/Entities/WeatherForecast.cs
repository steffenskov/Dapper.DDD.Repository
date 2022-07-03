using WeatherService.Model.Enums;

namespace WeatherService.Model.Entities;

public class WeatherForecast
{
	public long Id { get; private init; } // init because it's an identity field in the database, as such the code should never set it explicitly
	public int WeatherStationId { get; set; }
	public int TemperatureC { get; set; }
	public WeatherForecastSummary Summary { get; set; }
	public DateTime Timestamp { get; private init; } // init because it's being set by a default constraint in the database, as such the code should never set it explicitly
}
