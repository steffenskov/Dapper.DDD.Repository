using WeatherService.Domain.Enums;

namespace WeatherService.Api.Models.WeatherForecast;
public record WeatherForecastCreateModel(int WeatherStationId, int TemperatureC, WeatherForecastSummary Summary);
