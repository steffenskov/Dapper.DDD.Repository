using Dapper.Repository.DependencyInjection;
using Dapper.Repository.Sql;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Domain.Aggregates;
using WeatherService.Domain.Repositories;
using WeatherService.Infrastructure.Repositories;
using WeatherService.Infrastructure.Requirements;

namespace WeatherService.Infrastructure;

public static class Setup
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		return services.ConfigureDapperRepositoryDefaults(config =>
		{
			config.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			config.ConnectionFactory = new SqlConnectionFactory("Server=127.0.0.1;Database=WeatherService;User Id=sa;Password=SqlServerPassword#&%¤2019;Encrypt=False;");
			config.DapperInjectionFactory = new DapperInjectionFactory();
			config.Schema = "dbo";
		})
		.AddTableRepository<WeatherStation, int, IWeatherStationRepository, WeatherStationRepository>(config =>
		{
			config.HasKey(x => x.Id);
			config.HasIdentity(x => x.Id);
			config.TableName = "WeatherStations";
		})
		.AddTableRepository<WeatherForecast, long>(config =>
		{
			config.HasKey(x => x.Id);
			config.HasIdentity(x => x.Id);
			config.TableName = "WeatherForecasts";
			config.HasDefault(x => x.Timestamp);
		})
		.AddViewRepository<WeatherForecastView, IWeatherForecastViewRepository, WeatherForecastViewRepository>(config =>
		{
			config.ViewName = "WeatherForecastView";
		});
	}
}
