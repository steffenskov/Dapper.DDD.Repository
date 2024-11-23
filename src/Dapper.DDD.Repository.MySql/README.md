# Dapper.DDD.Repository.MySql

This package provides support for using Dapper.DDD.Repository with MySql and MariaDB.

# Usage

Install the package [MySql.Data](https://www.nuget.org/packages/MySql.Data) in your project.
Then simply provide an instance of `MySqlQueryGenerator` as your `QueryGenerator`, and create a `MySqlConnectionFactory`
in your project, and you're done:

```
	...
	options.ConnectionFactory = new MySqlConnectionFactory("CONNECTIONSTRING");
	options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
	...
```

Also here's a simple `MySqlConnectionFactory` you can use:

```
using System.Data;
using Dapper.DDD.Repository.Interfaces;
using MySql.Data.MySqlClient;

namespace YOUR_NAMESPACE_HERE;

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
```

# Documentation
Auto generated documentation via [DocFx](https://github.com/dotnet/docfx) is available here: https://steffenskov.github.io/Dapper.DDD.Repository/