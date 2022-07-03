using WeatherService.Domain.Repositories;

namespace WeatherService.Domain.Queries.WeatherForecast;

internal class WeatherForecastQueryHandler : IRequestHandler<WeatherForecastGetLatestQuery, IEnumerable<WeatherForecastView>>,
	IRequestHandler<WeatherForecastGetSingleQuery, WeatherForecastView?>
{
	private readonly IWeatherForecastViewRepository _viewRepository;

	public WeatherForecastQueryHandler(IWeatherForecastViewRepository viewRepository)
	{
		_viewRepository = viewRepository;
	}

	public async Task<IEnumerable<WeatherForecastView>> Handle(WeatherForecastGetLatestQuery request, CancellationToken cancellationToken)
	{
		return await _viewRepository.GetLatestAsync(cancellationToken);
	}

	public async Task<WeatherForecastView?> Handle(WeatherForecastGetSingleQuery request, CancellationToken cancellationToken)
	{
		return await _viewRepository.GetAsync(request.id, cancellationToken);
	}
}
