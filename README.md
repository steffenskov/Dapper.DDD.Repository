# Dapper.DDD.Repository

This is an extension library for the Dapper ORM, giving you simple-to-use repositories for all your database access
code.
It uses a Fluent syntax for configuring your repositories through the built-in Dependency Injection in .Net.

Also it's inspired by [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) and as such uses the
word `Aggregate` rather than `Entity` as well as the term `ValueObject`. It also allows you to configure everything
strictly outside your `Domain` layer, in order to keep the domain free from information about how your persistence
works. This makes it easier to replace the persistence, should you ever want to do that.

# Features

- FAST: When benchmarked against raw Dapper the CPU time difference is neglible. Feel free to run the benchmark project
  to see the numbers.
- Domain-Driven Design friendly with Fluent configuration outside the domain layer using DependencyInjection.
- Fully supports ValueObjects, and you can even infinitely nest them.
- Fully supports `record` types including the primary constructor syntax for very concise ValueObjects (or even
  Aggregates!)
- Built-in support for MS SqlServer, PostGreSql and MySql / MariaDB, easy to extend with support for other databases.
- Built-in CRUD methods
- Support for custom types via TypeConverters such as
  e.g. [StrongTypedId](https://github.com/steffenskov/StrongTypedId) (Which I also highly recommend using for
  Domain-Driven Design)
- Sample projects to help you get started.

## Installation:

I recommend using the NuGet package: https://www.nuget.org/packages/Dapper.DDD.Repository/ however you can also simply
clone the repository and compile the project yourself.

As the project is licensed under MIT you're free to use it for pretty much anything you want.

You also need to install Dapper yourself, again I'd recommend NuGet: https://www.nuget.org/packages/Dapper/

As for versioning of Dapper, you're actually free to choose whichever you want, as this library isn't built targetting a
specific version of Dapper.
Instead whatever Dapper version you prefer is injected into this extension library. This leaves you free to update
Dapper without waiting for a new version of this library.
The same goes for your database connection code, that too will be injected and you can run any version you like as long
as it can provide an `IDbConnection`.

If you're using Microsoft SQL Server you'll need to reference both the `Dapper.DDD.Repository` project as well as
the `Dapper.DDD.Repository.Sql` project.
Likewise for MySql you want `Dapper.DDD.Repository` and `Dapper.DDD.Repository.MySql`.

Finally if you want to utilize the built-in Dependency Injection in the newer versions of .Net, you'll want
the `Dapper.DDD.Repository.DependencyInjection` package too.

## Requirements:

The library requires .Net 6.

Also it currently only supports Microsoft SQL Server and MySql (MariaDB should work just fine with the MySql version
too), but feel free to branch it and create support for PostGre or whatever you're using (as long as Dapper supports it,
this library can too)

## Limitations:

Currently the library only supports tables with a primary key (no heap support), views are supported both with and
without including primary keys.

Also all the methods are kept `Async` and no synchronous versions are currently planned. This is because database
calls (like all I/O) should ideally be kept async for improved performance and responsiveness.

Finally if you want to utilize Dapper's SqlMapper functionality, you'll need to also instruct this instruction into
treating the type as a built-in type. Otherwise both Dapper and this extension will attempt to deal with the type
simultaneously.

An example:

```
SqlMapper.AddTypeHandler(new PolygonTypeMapper());

services.ConfigureDapperRepositoryDefaults(options =>
{
	options.TreatAsBuiltInType<Polygon>(); // Necessary to allow the SqlMapper to work its magic
}
```

## Usage:

In order to avoid building this library for a specific Dapper and database version, I've added injection points for
injecting the necessary Dapper extension methods as well as a `ConnectionFactory` into the repositories.
This requires a couple (3) of classes in your project to wire-up everything, but in return protects you from "dependency
version hell" :-)
So go ahead and create these 3 classes:

```
using System.Data;
using Dapper;
using Dapper.DDD.Repository.Interfaces;

namespace YOUR_NAMESPACE_HERE;

internal class DapperInjection<T> : IDapperInjection<T>
{
	public Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QueryAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T> QuerySingleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<object> QuerySingleAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<T?> QuerySingleOrDefaultAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleOrDefaultAsync<T?>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}

	public Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
	{
		return cnn.QuerySingleOrDefaultAsync(type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
	}
}
```

```
using Dapper.DDD.Repository.Interfaces;

namespace YOUR_NAMESPACE_HERE;

internal class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	{
		return new DapperInjection<T>();
	}
}
```

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
			throw new ArgumentException("connectionString cannot be null or whitespace.", nameof(connectionString));

		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		return new SqlConnection(_connectionString);
	}
}
```

The two `DapperInjection` classes are for injecting delegates to Dapper's extension methods into the Repository library.
The `SqlConnectionFactory` is for injecting the database connection. For MySql you'll want to create `MySqlConnection`s
instead.

That's the prerequisites taken care of, now onto actually using the library. For this example we're going to create a
very basic `UserRepository` mapping to a "Users" table looking like this:

```
CREATE TABLE Users
(
	UserId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Username VARCHAR(50) NOT NULL,
	Password VARCHAR(50) NOT NULL, // Don't store passwords in plain text please, this is just for illustration purposes
	Description VARCHAR(MAX) NULL,
	DateCreated DATETIME2(2) NOT NULL DEFAULT(GETUTCDATE())
)
```

Your Aggregate class is now going to look like this (note: `record` is also fully supported instead of `class` if you
want):

```
public class User
{
	public int UserId { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
	public string? Description { get; set; } // If you're not using the new nullability feature, just remove the questionmark
	public DateTime DateCreated { get; } // No setter because of the database default constraint - we don't want to ever change this property in code.
}
```

And finally to configure the repository you'll want to configure the dependency injection in `Startup.cs`, `Program.cs`
or wherever you're doing DI configuration in your project:

```
	services.ConfigureDapperRepositoryDefaults(options =>
	{
		options.ConnectionFactory = new SqlConnectionFactory("CONNECTIONSTRING"); // Note: Connectionstring should probably come from configuration rather than being hardcoded here
		options.DapperInjectionFactory = new DapperInjectionFactory();
		options.QueryGeneratorFactory = new SqlQueryGeneratorFactory(); // Use MySqlQueryGeneratorFactory() if using MySql
		options.Schema = "dbo"; // Default schema, don't use this for MySql as it doesn't have the concept of schemas that SQL Server does.
		options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveValue, primitiveValue => new CategoryId(primitiveValue)); // Example based on StrongTypedId
	});
	services.AddTableRepository<User, int>(options => // The generic types are <TAggregate, TAggregateId>
	{
		options.TableName = "Users";
		options.HasKey(user => user.UserId);
		options.HasIdentity(user => user.UserId);
	});
```

From here on you can inject an `ITableRepository<User, int>` anywhere with the built-in Dependency-Injection.

If you need more functionality than basic CRUD, simply create your own interface that
implements `ITableRepository<TAggregate, TAggregateId>` as well as a your own class that implements your interface and
inherits `TableRepository<TAggregate, TAggregateId>`. (Note: an `IViewRepository` interface and `ViewRepository` class
is available as well for your SQL view needs)

Here's an example:

```
public interface IUserRepository : ITableRepository<User, int>
{
	// Whatever extra methods you need, e.g.
	Task<IEnumerable<User>> GetUsersWithoutPasswordAsync(CancellationToken cancellationToken);
}
```

```
public class UserRepository : TableRepository<User, int>, IUserRepository
{
	public UserRepository(IOptions<TableAggregateConfiguration<User>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task<IEnumerable<User>> GetUsersWithoutPasswordAsync(CancellationToken cancellationToken)
	{
		return await QueryAsync($"SELECT {PropertyList} FROM {TableName} WHERE Name = @name", new { name }, cancellationToken: cancellationToken);
	}
}
```

And finally configure it with dependency injection like this instead:

```
services.AddTableRepository<User, int, IUserRepository, UserRepository>(options =>
	{
		options.TableName = "Users";
		options.HasKey(user => user.UserId);
		options.HasIdentity(user => user.UserId);
	});
```

From here on you can inject an `IUserRepository` anywhere with the built-in Dependency-Injection.

## Upcoming features

- Improvements to AggregateConfiguration injection, as the current "explicit interface" approach is a bit annoying for
  when adding support for new databases.