namespace Dapper.DDD.Repository.IntegrationTests;

public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	where T : notnull
	{
		return new DapperInjection<T>();
	}
}