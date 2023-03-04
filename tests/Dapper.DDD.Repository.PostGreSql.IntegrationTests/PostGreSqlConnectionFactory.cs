using System.Data;
using Dapper.DDD.Repository.Interfaces;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PostGreSqlConnectionFactory : IConnectionFactory
{
	private readonly string _connectionString;

	public PostGreSqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("Connectionstring cannot be null or whitespace.", nameof(connectionString));
		}

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new NpgsqlConnection(_connectionString);
	}
}