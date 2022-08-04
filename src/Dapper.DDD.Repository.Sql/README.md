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

## Using Geometry and Geography

Dapper doesn't inherently play that well with these column types, however this package has a solution for it.
In this example I'll use [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) for my C# `Geometry` classes, but the principle is the same if you're using Microsoft's classes (or even some other third party).

The solution is actually rather simple: Serialize Geometry/Geography columns to binary, as this is supported by Dapper. To do so requires two configurations:
- The `SqlQueryGeneratorFactory` needs to know which types to serialize in order to generate the proper SQL (which looks like this: `([dbo].[Table].[Column]).Serialize() AS [Column]`)
- `TypeConverters` needs to be added for actually converting to/from binary

Here's an example of such a configuration for the `Point` type from `NetTopologySuite` - use the same approach for any other types:

```
	using NetTopologySuite.Geometries;
	using NetTopologySuite.IO;
	...
	_ = services.ConfigureDapperRepositoryDefaults(options =>
	{
		...
		options.QueryGeneratorFactory = new SqlQueryGeneratorFactory().SerializeColumnType(type => type.Namespace == "NetTopologySuite.Geometries"); // This ensure any Geometry type from NetTopologySuite gets serialized in the queries
		options.AddTypeConverter<Point, byte[]>(geo => new SqlServerBytesWriter() { IsGeography = false }.Write(geo), bytes => (Point)new SqlServerBytesReader() { IsGeography = false }.Read(bytes)); // Set IsGeography to true if you're using geography and not geometry
		...
	}
```

And that's basically it, now you can define properties of the type `Point` in any of your aggregates and the built-in `ITableRepository` will work correctly.

There are however still 2 caveats to look out for:
- You need to remember to `.Serialize()` your column manually for your own queries, otherwise you'll get an exception similar to this: `System.Data.DataException : Error parsing column`
- There is currently no way to support both geography and geometry of the same C# type simultaneously. This is because only a single `TypeConverter` can exist for a single type. A workaround **might** be to declare two classes yourself and use those instead like this:
```
	public class GeometryPoint : Point
	{
		...
	}

	public class GeographyPoint : Point
	{
		...
	}
	...
	options.AddTypeConverter<GeometryPoint, byte[]>(geo => new SqlServerBytesWriter() { IsGeography = false }.Write(geo), bytes => (GeometryPoint)new SqlServerBytesReader() { IsGeography = false }.Read(bytes));
	options.AddTypeConverter<GeographyPoint, byte[]>(geo => new SqlServerBytesWriter() { IsGeography = true }.Write(geo), bytes => (GeographyPoint)new SqlServerBytesReader() { IsGeography = true }.Read(bytes));
```

**Note**: I haven't tested this approach myself, so it might not be that simple! It depends on whether the `SqlServerBytesReader` can deal with custom types or not.

### Known issues with Geometry and Geography

These 2 exceptions will appear if you've missed one of the steps above:
- `System.Data.DataException : Error parsing column`: You've not added `.SerializeColumnType` to your `SqlQueryGeneratorFactory`, it's not correctly detecting the type or you've forgotten to call `.Serialize()` in your own custom Query.
- `System.ArgumentException : An item with the same key has already been added. Key: Item`: You've not added a `TypeConverter` for one or more of your C# geometry types.