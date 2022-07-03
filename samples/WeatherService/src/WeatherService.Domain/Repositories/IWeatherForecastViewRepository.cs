namespace WeatherService.Domain.Repositories
{
	public interface IWeatherForecastViewRepository
	{
		Task<IEnumerable<WeatherForecastView>> GetLatestAsync(CancellationToken cancellationToken);
		Task<WeatherForecastView?> GetAsync(long id, CancellationToken cancellationToken);
	}
}
