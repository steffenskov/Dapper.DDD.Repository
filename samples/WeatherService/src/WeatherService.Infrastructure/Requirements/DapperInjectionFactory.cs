using Dapper.DDD.Repository.Interfaces;

namespace WeatherService.Infrastructure.Requirements;
public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	{
		return new DapperInjection<T>();
	}
}
