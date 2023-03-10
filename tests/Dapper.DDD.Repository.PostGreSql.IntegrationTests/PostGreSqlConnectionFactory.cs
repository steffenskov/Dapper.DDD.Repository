using System.Data;
using Dapper.DDD.Repository.Interfaces;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PostGreSqlConnectionFactory : IConnectionFactory
{
	private readonly NpgsqlDataSource _dataSource;

	public PostGreSqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("connectionString cannot be null or whitespace.", nameof(connectionString));
		}

		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
		dataSourceBuilder.UseNetTopologySuite();
		_dataSource = dataSourceBuilder.Build();
	}

	~PostGreSqlConnectionFactory()
	{
		_dataSource.Dispose();
	}

	public IDbConnection CreateConnection()
	{
		return _dataSource.CreateConnection();
	}
}