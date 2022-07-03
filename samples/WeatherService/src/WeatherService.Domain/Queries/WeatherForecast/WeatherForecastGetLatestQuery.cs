namespace WeatherService.Domain.Queries.WeatherForecast;

public record WeatherForecastGetLatestQuery() : IRequest<IEnumerable<WeatherForecastView>>;
