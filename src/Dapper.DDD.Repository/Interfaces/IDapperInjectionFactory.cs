namespace Dapper.DDD.Repository.Interfaces;

public interface IDapperInjectionFactory
{
	IDapperInjection<T> Create<T>()
	where T : notnull;
}
