using System.Data;
using Dapper.DDD.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace WeatherService.Infrastructure.Requirements;

public class SqlConnectionFactory : IConnectionFactory
{
	private readonly string _connectionString;

	public SqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("Connectionstring cannot be null or whitespace.", nameof(connectionString));
		}

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new SqlConnection(_connectionString);
	}
}
