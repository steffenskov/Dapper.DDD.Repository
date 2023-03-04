# Dapper.DDD.Repository.PostGreSql

This package provides support for using Dapper.DDD.Repository with PostGreSql.

# Usage

Install the package [Npgsql](https://www.nuget.org/packages/Npgsql) in your project.
Then simply provide an instance of `PostGreSqlQueryGenerator` as your `QueryGenerator`, and create a `PostGreSqlConnectionFactory`
in your project, and you're done:

```
	...
	options.ConnectionFactory = new PostGreSqlConnectionFactory("CONNECTIONSTRING");
	options.QueryGeneratorFactory = new PostGreSqlQueryGeneratorFactory();
	...
```

Also here's a simple `PostGreSqlConnectionFactory` you can use:

```
using System.Data;
using Dapper.DDD.Repository.Interfaces;
using Npgsql;

namespace YOUR_NAMESPACE_HERE;

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
```