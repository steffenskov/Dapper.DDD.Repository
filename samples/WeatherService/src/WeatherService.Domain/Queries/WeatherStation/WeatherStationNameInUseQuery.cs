namespace WeatherService.Domain.Queries.WeatherStation;

internal record WeatherStationNameInUseQuery(string name) : IRequest<bool>;
