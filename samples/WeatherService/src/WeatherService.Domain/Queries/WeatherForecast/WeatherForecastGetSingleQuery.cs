namespace WeatherService.Domain.Queries.WeatherForecast;

public record WeatherForecastGetSingleQuery(long id) : IRequest<WeatherForecastView?>;
