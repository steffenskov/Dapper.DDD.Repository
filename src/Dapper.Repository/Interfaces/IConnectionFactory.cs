namespace Dapper.Repository.Interfaces;

public interface IConnectionFactory
{
	IDbConnection CreateConnection();
}