using WeatherService.Api.Models.WeatherStation;
using WeatherService.Domain.Aggregates;
using WeatherService.Domain.Enums;

namespace WeatherService.Api.Models.WeatherForecast;

public record WeatherForecastViewModel(long Id, WeatherStationViewModel WeatherStation, int TemperatureC, WeatherForecastSummary Summary, DateTime Timestamp)
{
	public static WeatherForecastViewModel FromDomainModel(WeatherForecastView weatherForecast)
	{
		return new WeatherForecastViewModel(weatherForecast.Id, WeatherStationViewModel.FromDomainModel(weatherForecast.WeatherStation), weatherForecast.TemperatureC, weatherForecast.Summary, weatherForecast.Timestamp);
	}
}
