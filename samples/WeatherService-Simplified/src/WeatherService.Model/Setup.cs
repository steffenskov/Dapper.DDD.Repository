using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.Sql;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Model.Entities;
using WeatherService.Model.Repositories;
using WeatherService.Model.Requirements;

namespace WeatherService.Model;

public static class Setup
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
	{
		return services.ConfigureDapperRepositoryDefaults(config =>
		{
			config.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			config.ConnectionFactory = new SqlConnectionFactory(connectionString);
			config.DapperInjectionFactory = new DapperInjectionFactory();
			config.Schema = "dbo";
		})
		.AddTableRepository<WeatherForecast, long>(config => // Since we currently don't need anything other than basic CRUD here, no custom repository implementation has been made
		{
			config.HasKey(weatherForecast => weatherForecast.Id);
			config.HasIdentity(weatherForecast => weatherForecast.Id);
			config.TableName = "WeatherForecasts";
			config.HasDefault(weatherForecast => weatherForecast.Timestamp);
		})
		.AddTableRepository<WeatherStation, int, IWeatherStationRepository, WeatherStationRepository>(config => // We use a custom repository for IWeatherStation as more methods than the basic CRUD ones are needed
		{
			config.HasKey(weatherStation => weatherStation.Id);
			config.HasIdentity(weatherStation => weatherStation.Id);
			config.TableName = "WeatherStations";
		})
		.AddViewRepository<WeatherForecastView, IWeatherForecastViewRepository, WeatherForecastViewRepository>(config => // Since one requires custom methods as well
		{
			config.ViewName = "WeatherForecastView";
		});
	}
}
