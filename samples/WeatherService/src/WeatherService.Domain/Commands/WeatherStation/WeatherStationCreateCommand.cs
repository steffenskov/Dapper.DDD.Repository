namespace WeatherService.Domain.Commands.WeatherStation;

public record WeatherStationCreateCommand(string Name, Address Address) : IRequest<Aggregates.WeatherStation>;
