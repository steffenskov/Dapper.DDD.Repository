using WeatherService.Model.Entities;

namespace WeatherService.Model.Repositories;

/// <summary>
/// We don't need the built-in "GetAllAsync" method from IViewRepository, and as such can keep this interface simpler by not implementing IViewRepository at all.
/// </summary>
public interface IWeatherForecastViewRepository
{
	Task<IEnumerable<WeatherForecastView>> GetLatestAsync(CancellationToken cancellationToken);
	Task<WeatherForecastView?> GetAsync(long id, CancellationToken cancellationToken);
}
