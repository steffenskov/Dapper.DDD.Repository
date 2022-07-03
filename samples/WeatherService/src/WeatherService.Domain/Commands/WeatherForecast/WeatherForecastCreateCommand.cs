using WeatherService.Domain.Enums;

namespace WeatherService.Domain.Commands.WeatherForecast;

public record WeatherForecastCreateCommand(int WeatherStationId, int TemperatureC, WeatherForecastSummary Summary) : IRequest<WeatherForecastView>;
