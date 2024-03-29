using System.Data;
using Dapper.DDD.Repository.Interfaces;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class MySqlConnectionFactory : IConnectionFactory
{
	private readonly string _connectionString;

	public MySqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("connectionString cannot be null or whitespace.", nameof(connectionString));
		}

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new MySqlConnection(_connectionString);
	}
}