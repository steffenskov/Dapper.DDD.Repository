using Dapper.Repository.Interfaces;

namespace Dapper.Repository.IntegrationTests;

public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	where T : notnull
	{
		return new DapperInjection<T>();
	}
}