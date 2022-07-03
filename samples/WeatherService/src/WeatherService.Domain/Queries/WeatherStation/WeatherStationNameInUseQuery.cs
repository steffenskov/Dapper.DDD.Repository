namespace WeatherService.Domain.Queries.WeatherStation;

internal record WeatherStationNameInUseQuery(string Name) : IRequest<bool>;
