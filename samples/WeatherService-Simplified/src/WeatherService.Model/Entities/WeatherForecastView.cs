using WeatherService.Model.Enums;

namespace WeatherService.Model.Entities
{
	public class WeatherForecastView
	{
		public long Id { get; private init; } // private init for everything as a view is of course read-only
		public int TemperatureC { get; private init; }
		public WeatherForecastSummary Summary { get; private init; }
		public DateTime Timestamp { get; private init; }
		public WeatherStation WeatherStation { get; private init; } = default!; // Here the WeatherStation aggregate is actually used as a ValueObject
	}
}
