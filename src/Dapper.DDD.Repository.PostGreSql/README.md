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
			throw new ArgumentException("connectionString cannot be null or whitespace.", nameof(connectionString));
		}

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new NpgsqlConnection(_connectionString);
	}
}
```

## Using Geometry and Geography

Dapper doesn't inherently play that well with these column types, however this package has a solution for it.
In this example I'll use [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) for my C# `Geometry`
classes, but the principle is the same if you're using Microsoft's classes (or even some other third party).

Apart from [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) you'll want to install the [Npgsql.NetTopologySuite](https://www.nuget.org/packages/Npgsql.NetTopologySuite) package as well.

This solution uses the PostGIS extension in your PostGreSql database and contains a few steps.

Assume a table looking like this:
```
CREATE TABLE cities
(
	Id UUID NOT NULL PRIMARY KEY,
	Name char varying(200) NOT NULL,
	GeoLocation GEOMETRY(POINT, 25832) NOT NULL
);
```

And it's equivalent C# class looking like this:
```
public record City
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public Point GeoLocation { get; set; }
}
```

We'll need to do the following to enable this:

### Connection Factory
```
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
```

This allows us to call `UseNetTopologySuite` which is crucial.
It also introduces a finalizer for cleaning up the datasource, as `IDisposable` isn't really feasible with the Factory classes at the moment.

### Dapper Type Mappers

In this example I'll use a mapper for `Point`, but feel free to add more as you introduce more types:

```
public class PointTypeMapper : SqlMapper.TypeHandler<Point>
{
	public override void SetValue(IDbDataParameter parameter, Point value)
	{
		parameter.Value = (object?)value?.AsBinary() ?? DBNull.Value;
		parameter.DbType = DbType.Binary;
	}

	public override Point Parse(object? value)
	{
		if (value is null or DBNull)
			return null!;

		return (Point)value;
	}
}
```

### Configuration

The final step is to configure Dapper and the repository extension:

```
SqlMapper.AddTypeHandler(new PointTypeMapper());
...

services.ConfigureDapperRepositoryDefaults(options =>
{
	options.ConnectionFactory = new PostGreSqlConnectionFactory(connectionString);
	options.DapperInjectionFactory = new DapperInjectionFactory();
	options.QueryGeneratorFactory = new PostGreSqlQueryGeneratorFactory();
	options.TreatAsBuiltInType<Point>(); // Necessary to allow the SqlMapper to work its magic
	...
});
```

This does two things:
- Configures Dapper to use the PointTypeMapper we've defined
- Configures the Repository extension to treat the `Pont` type as a built-in type, which means all the "automagic" functionality of the Repository extension is disabled and Dapper gets to do its thing alone (which is to use the PointTypeMapper)

~~~~
And that's basically it, now you can define properties of the type `Point` in any of your aggregates and the
built-in `ITableRepository` will work correctly.

# Documentation
Auto generated documentation via [DocFx](https://github.com/dotnet/docfx) is available here: https://steffenskov.github.io/Dapper.DDD.Repository/