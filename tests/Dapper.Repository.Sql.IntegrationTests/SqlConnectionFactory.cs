using System.Data;
using Dapper.Repository.Interfaces;

namespace Dapper.Repository.Sql.IntegrationTests;

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
