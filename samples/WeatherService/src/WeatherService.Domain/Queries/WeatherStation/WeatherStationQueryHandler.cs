using WeatherService.Domain.Repositories;

namespace WeatherService.Domain.Queries.WeatherStation;

internal class WeatherStationQueryHandler : IRequestHandler<WeatherStationNameInUseQuery, bool>,
	IRequestHandler<WeatherStationGetAllQuery, IEnumerable<Aggregates.WeatherStation>>,
	IRequestHandler<WeatherStationGetSingleQuery, Aggregates.WeatherStation?>
{
	private readonly IWeatherStationRepository _repository;

	public WeatherStationQueryHandler(IWeatherStationRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> Handle(WeatherStationNameInUseQuery request, CancellationToken cancellationToken)
	{
		return await _repository.NameInUseAsync(request.Name, cancellationToken);
	}

	public async Task<IEnumerable<Aggregates.WeatherStation>> Handle(WeatherStationGetAllQuery request, CancellationToken cancellationToken)
	{
		return await _repository.GetAllAsync(cancellationToken);
	}

	public async Task<Aggregates.WeatherStation?> Handle(WeatherStationGetSingleQuery request, CancellationToken cancellationToken)
	{
		return await _repository.GetAsync(request.Id, cancellationToken);
	}
}
