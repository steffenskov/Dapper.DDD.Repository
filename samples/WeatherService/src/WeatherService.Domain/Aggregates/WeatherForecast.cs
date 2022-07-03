using WeatherService.Domain.Enums;

namespace WeatherService.Domain.Aggregates;

public class WeatherForecast
{
	public long Id { get; private init; } // init because it's an identity field in the database, as such the code should never set it explicitly
	public int WeatherStationId { get; private set; }
	public int TemperatureC { get; private set; }
	public WeatherForecastSummary Summary { get; private set; }
	public DateTime Timestamp { get; private init; } // init because it's being set by a default constraint in the database, as such the code should never set it explicitly
}
