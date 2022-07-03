using WeatherService.Api.Models.WeatherStation;
using WeatherService.Model.Entities;
using WeatherService.Model.Enums;

namespace WeatherService.Api.Models.WeatherForecast;

public record WeatherForecastViewModel(long Id, WeatherStationViewModel WeatherStation, int TemperatureC, WeatherForecastSummary Summary, DateTime Timestamp)
{
	public static WeatherForecastViewModel FromEntity(WeatherForecastView weatherForecast)
	{
		return new WeatherForecastViewModel(weatherForecast.Id, WeatherStationViewModel.FromEntity(weatherForecast.WeatherStation), weatherForecast.TemperatureC, weatherForecast.Summary, weatherForecast.Timestamp);
	}
}
