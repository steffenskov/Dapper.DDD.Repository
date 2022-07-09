using Dapper.DDD.Repository.Interfaces;

namespace WeatherService.Domain.Repositories;

/// <summary>
/// The IWeatherStationRepository expands upon the functionality that's already in the ITableRepository definition.
/// However if you'd rather not include a reference to the Dapper.DDD.Repository library in your Domain layer, you can simply remove the implementation of ITableRepository.
/// You'll just have to define whatever CRUD operations you need yourself then.
/// </summary>
public interface IWeatherStationRepository : ITableRepository<WeatherStation, int>
{
	Task<bool> NameInUseAsync(string name, CancellationToken cancellationToken);
}
