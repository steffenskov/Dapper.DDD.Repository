using Dapper.DDD.Repository.Interfaces;

namespace WeatherService.Model.Requirements;
public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	{
		return new DapperInjection<T>();
	}
}
