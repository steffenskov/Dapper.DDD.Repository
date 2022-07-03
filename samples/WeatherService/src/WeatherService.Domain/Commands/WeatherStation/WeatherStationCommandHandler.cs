namespace WeatherService.Domain.Commands.WeatherStation;

internal class WeatherStationCommandHandler : IRequestHandler<WeatherStationCreateCommand, Aggregates.WeatherStation>
{
	public WeatherStationCommandHandler()
	{

	}

	public Task<Aggregates.WeatherStation> Handle(WeatherStationCreateCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
