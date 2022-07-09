namespace Dapper.DDD.Repository.Interfaces;

public interface IConnectionFactory
{
	IDbConnection CreateConnection();
}