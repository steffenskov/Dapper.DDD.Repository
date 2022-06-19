# Dapper.Repository
This is an extension library for the Dapper ORM, giving you simple-to-use repositories for all your database access code.
It uses a Fluent syntax for configuring your repositories through the built-in Dependency Injection in .Net.

Also it's somewhat inspired by [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) and as such uses the word `Aggregate` rather than `Entity` as well as the term `ValueObject`. It also allows you to configure everything strictly outside your `Domain` layer, in order to keep the domain free from information about how your persistance works. This makes it easier to replace the persistance, should you ever want to do that.

**NOTICE**: The current codebase is still very much "work-in-progress", as such these features are still not fully implemented, but are being worked on:
- Prefixing ValueObject columns in the database (e.g. Address_Road, Address_Zipcode, Address_City)
- Cleaning up in general
- Performance optimization of property lists (will be changed into dictionaries for O(1) lookups in the future)
- Integration tests of IViewRepository
- Abstracting dealing with ValueObjects away from the end user
- Improvements to AggregateConfiguration injection, as the current "explicit interface" approach is a bit annoying for when adding support for new databases.

## Installation:

~~I recommend using the NuGet package: https://www.nuget.org/packages/Dapper.Repository/ however you can also simply clone the repository and compile the project yourself.~~

The package will be available on NuGet once version 1.0 is ready, until then feel free to clone the repo and play around with it from source.

As the project is licensed under MIT you're free to use it for pretty much anything you want.

You also need to install Dapper yourself, again I'd recommend NuGet: https://www.nuget.org/packages/Dapper/

As for versioning of Dapper, you're actually free to choose whichever you want, as this library isn't built targetting a specific version of Dapper. 
Instead whatever Dapper version you prefer is injected into this extension library. This leaves you free to update Dapper without waiting for a new version of this library.
The same goes for your database connection code, that too will be injected and you can run any version you like as long as it can provide an `IDbConnection`.

If you're using Microsoft SQL Server you'll need to reference both the `Dapper.Repository` project as well as the `Dapper.Repository.Sql` project.
Likewise for MySql you want `Dapper.Repository` and `Dapper.Repository.MySql`.

## Requirements:

The library requires .Net 6.

Also it currently only supports Microsoft SQL Server and MySql (MariaDB should work just fine with the MySql version too), but feel free to branch it and create support for PostGre or whatever you're using (as long as Dapper supports it, this library can too)

## Limitations:

Currently the library only supports tables with a primary key (no heap support), views are supported both with and without primary keys.
Also all the methods are kept `Async` and no synchronous versions are currently planned. This is because database calls (like all I/O) should probably be kept async for improved performance and responsiveness.
Finally there's currently no support for `CancellationToken` as the latest version of Dapper doesn't support this when dealing with `ValueObjects`. I'll keep an eye out for updates to Dapper in this regard, and add support as soon as Dapper has it.

## Usage:

In order to avoid building this library for a specific Dapper version, I've added an injection point for injecting the necessary Dapper extension methods into the repositories.  
This requires a couple (3) of classes in your project to wire-up everything.
So go ahead and create these 3 classes:

```
using System.Data;
using Dapper.Repository.Interfaces;

namespace YOUR_NAMESPACE_HERE;

public class DapperInjection<T> : IDapperInjection<T>
where T : notnull
{
	public Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		return cnn.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout, commandType));
	}

	public Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		return cnn.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType));
	}

	public Task<IEnumerable<T>> QueryAsync(IDbConnection cnn, string sql, Type[] types, Func<object[], T> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
	{
		return cnn.QueryAsync<T>(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
	}

	public Task<T> QuerySingleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		return cnn.QuerySingleAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType));
	}

	public Task<T?> QuerySingleOrDefaultAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
	{
		return cnn.QuerySingleOrDefaultAsync<T?>(new CommandDefinition(sql, param, transaction, commandTimeout, commandType));
	}
}
```

```
using Dapper.Repository.Interfaces;

namespace YOUR_NAMESPACE_HERE;

public class DapperInjectionFactory : IDapperInjectionFactory
{
	public IDapperInjection<T> Create<T>()
	where T : notnull
	{
		return new DapperInjection<T>();
	}
}
```

```
using System.Data;
using Dapper.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace YOUR_NAMESPACE_HERE;

public class SqlConnectionFactory : IConnectionFactory
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

The two `DapperInjection` classes are for injecting delegates to Dapper's extension methods into the Repository library. The `SqlConnectionFactory` is for injecting the database connection. For MySql you'll want to create `MySqlConnection`s instead.


That's the prerequisites taken care of, now onto actually using the library. For this example we're going to create a very basic UserRepository mapping to a "Users" table looking like this:

```
CREATE TABLE Users
(
	UserID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Username VARCHAR(50) NOT NULL,
	Password VARCHAR(50) NOT NULL, // Don't store passwords in plain text please, this is just for illustration purposes
	Description VARCHAR(MAX) NULL,
	DateCreated DATETIME() NOT NULL DEFAULT(GETDATE())
)
```

Your Aggregate class is now going to look like this (note: `record` is also fully supported instead of class if you want):

```
public class User
{
	public int UserID { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
	public string? Description { get; set; } // If you're not using the new nullability feature, just remove the questionmark
	public DateTime DateCreated { get; private set; } // Doesn't need a public setter it has a DB default constraint
}
```

And finally to configure the repository you'll want to configure the dependency injection in `Startup.cs`, `Program.cs` or wherever you're doing DI configuration in your project:

```
	services.ConfigureDapperRepositoryDefaults(options =>
	{
		options.ConnectionFactory = new SqlConnectionFactory("CONNECTIONSTRING"); // Note: Connectionstring should probably come from configuration rather than being hardcoded here
		options.DapperInjectionFactory = new DapperInjectionFactory();
		options.QueryGeneratorFactory = new SqlQueryGeneratorFactory(); // Use MySqlQueryGeneratorFactory() if using MySql
		options.Schema = "dbo"; // Default schema, don't use this for MySql as it doesn't have the concept of schemas that SQL Server does.
	});
	services.AddTableRepository<User, int>(options => // The generic types are <Aggregate, ID>
	{
		options.TableName = "Users";
		options.HasKey(user => user.UserID);
		options.HasIdentity(user => user.UserID);
	});
```

From here on you can inject an `ITableRepository<User, int>` anywhere with the built-in Dependency-Injection.

At the moment only aggregating repositories are supported (as opposed to inheritance), so if you need more than the CRUD functionality provided by `ITableRepository`, I'd suggest creating your own repository similar to this:

```
public interface IUserRepository
{
	// Whatever methods you need, e.g.
	Task<User> CreateUserAsync(User user);
	Task<IEnumerable<User>> GetUsersWithoutPasswordAsync();
}
```

```
public class UserRepository : IUserRepository
{
	private ITableRepository<User, int> _underlyingRepository;

	public UserRepository(ITableRepository<User, int> underlyingRepository)
	{
		_underlyingRepository = underlyingRepository;
	}

	public async Task<User> CreateUserAsync(User user)
	{
		return await _underlyingRepository.InsertAsync(user);
	}

	public async Task<IEnumerable<User>> GetUsersWithoutPasswordAsync()
	{
		return await _underlyingRepository.QueryAsync("SELECT * FROM Users WHERE Password = '';);
	}
}
```
