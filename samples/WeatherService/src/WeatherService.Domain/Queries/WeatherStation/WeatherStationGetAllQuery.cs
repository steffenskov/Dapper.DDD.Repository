namespace WeatherService.Domain.Queries.WeatherStation;

public record WeatherStationGetAllQuery() : IRequest<IEnumerable<Aggregates.WeatherStation>>;
