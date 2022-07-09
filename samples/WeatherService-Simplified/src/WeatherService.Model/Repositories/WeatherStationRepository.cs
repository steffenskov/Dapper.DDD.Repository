using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.Options;
using WeatherService.Model.Entities;
using WeatherService.Model.Repositories;

namespace WeatherService.Model.Repositories
{
	internal class WeatherStationRepository : TableRepository<WeatherStation, int>, IWeatherStationRepository
	{
		public WeatherStationRepository(IOptions<TableAggregateConfiguration<WeatherStation>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
		{
		}

		public async Task<bool> NameInUseAsync(string name, CancellationToken cancellationToken)
		{
			return await ScalarSingleOrDefaultAsync<bool>($"SELECT TOP 1 CAST(1 as BIT) FROM {TableName} WHERE Name = @name", new { name }, cancellationToken: cancellationToken);
		}
	}
}
