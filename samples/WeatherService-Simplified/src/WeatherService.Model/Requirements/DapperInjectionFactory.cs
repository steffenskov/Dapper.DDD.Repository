using Dapper.Repository.Interfaces;

namespace WeatherService.Model.Requirements;
public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	where T : notnull
	{
		return new DapperInjection<T>();
	}
}
