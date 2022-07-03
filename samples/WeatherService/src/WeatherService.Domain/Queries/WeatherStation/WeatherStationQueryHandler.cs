namespace WeatherService.Domain.Queries.WeatherStation;

internal class WeatherStationQueryHandler : IRequestHandler<WeatherStationNameInUseQuery, bool>
{
	public WeatherStationQueryHandler()
	{

	}

	public Task<bool> Handle(WeatherStationNameInUseQuery request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
