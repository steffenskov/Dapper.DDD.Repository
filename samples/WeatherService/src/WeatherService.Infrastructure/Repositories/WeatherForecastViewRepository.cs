using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.Options;
using WeatherService.Domain.Aggregates;
using WeatherService.Domain.Repositories;

namespace WeatherService.Infrastructure.Repositories
{
	internal class WeatherForecastViewRepository : ViewRepository<WeatherForecastView>, IWeatherForecastViewRepository
	{
		public WeatherForecastViewRepository(IOptions<ViewAggregateConfiguration<WeatherForecastView>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
		{
		}

		public async Task<WeatherForecastView?> GetAsync(long id, CancellationToken cancellationToken)
		{
			return await QuerySingleOrDefaultAsync($"SELECT {PropertyList} FROM {ViewName} WHERE Id = @id", new { id }, cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<WeatherForecastView>> GetLatestAsync(CancellationToken cancellationToken)
		{
			return await QueryAsync(@$"WITH
  CTE AS(
    SELECT
      WeatherStation_id,
      Max(Timestamp) as LatestTimestamp
    FROM
      {ViewName}
    GROUP BY
      WeatherStation_id
  )
SELECT
  {ViewName}.*
FROM
  {ViewName}
  INNER JOIN CTE ON CTE.WeatherStation_id = {ViewName}.WeatherStation_id
  AND CTE.LatestTimestamp = {ViewName}.Timestamp", cancellationToken: cancellationToken);
		}
	}
}
