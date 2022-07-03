using Dapper.Repository.Configuration;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.Options;
using WeatherService.Model.Entities;
using WeatherService.Model.Repositories;

namespace WeatherService.Model.Repositories
{
	internal class WeatherForecastViewRepository : ViewRepository<WeatherForecastView>, IWeatherForecastViewRepository
	{
		public WeatherForecastViewRepository(IOptions<ViewAggregateConfiguration<WeatherForecastView>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
		{
		}

		public async Task<WeatherForecastView?> GetAsync(long id, CancellationToken cancellationToken)
		{
			return await QuerySingleOrDefaultAsync("SELECT * FROM WeatherForecastView WHERE Id = @id", new { id }, cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<WeatherForecastView>> GetLatestAsync(CancellationToken cancellationToken)
		{
			return await QueryAsync(@"WITH
  CTE AS(
    SELECT
      WeatherStation_id,
      Max(Timestamp) as LatestTimestamp
    FROM
      WeatherForecastView
    GROUP BY
      WeatherStation_id
  )
SELECT
  WeatherForecastView.*
FROM
  WeatherForecastView
  INNER JOIN CTE ON CTE.WeatherStation_id = WeatherForecastView.WeatherStation_id
  AND CTE.LatestTimestamp = WeatherForecastView.Timestamp", cancellationToken: cancellationToken);
		}
	}
}
