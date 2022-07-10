# Dapper.DDD.Repository.Sql

This package provides support for using Dapper.DDD.Repository with MS SQL Server.

# Usage

Install the package [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) in your project.
Then simply provide an instance of `SqlQueryGenerator` as your `QueryGenerator`, set your `Schema` and create a `SqlConnectionFactory` in your project, and you're done:

```
	...
	options.ConnectionFactory = new SqlConnectionFactory("CONNECTIONSTRING");
	options.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
	options.Schema = "dbo"; // Default schema, adjust if necessary
	...
```

Also here's a simple `SqlConnectionFactory` you can use:

```
using System.Data;
using Dapper.DDD.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace YOUR_NAMESPACE_HERE;

internal class SqlConnectionFactory : IConnectionFactory
{
	private readonly string _connectionString;

	public SqlConnectionFactory(string connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
			throw new ArgumentException("Connectionstring cannot be null or whitespace.", nameof(connectionString));

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new SqlConnection(_connectionString);
	}
}
```