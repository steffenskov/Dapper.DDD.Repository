namespace Dapper.Repository.Interfaces;

public interface IDapperInjectionFactory
{
	IDapperInjection<T> Create<T>()
	where T : notnull;
}
