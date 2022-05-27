using Dapper.Repository.Interfaces;

namespace Dapper.Repository.IntegrationTests;

public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	{
		return new DapperInjection<T>();
	}
}