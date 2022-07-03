namespace WeatherService.Domain.Queries.WeatherStation;

public record WeatherStationGetSingleQuery(int Id) : IRequest<Aggregates.WeatherStation?>;
