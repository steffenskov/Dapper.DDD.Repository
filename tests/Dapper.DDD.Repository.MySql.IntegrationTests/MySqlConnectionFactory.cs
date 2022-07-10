using System.Data;
using Dapper.DDD.Repository.Interfaces;
using MySql.Data.MySqlClient;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class MySqlConnectionFactory : IConnectionFactory
{
	private readonly string _connectionString;

	public MySqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("Connectionstring cannot be null or whitespace.", nameof(connectionString));
		}

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new MySqlConnection(_connectionString);
	}
}
