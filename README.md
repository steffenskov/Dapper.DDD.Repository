# Dapper.Repository
This is an extension library for the Dapper ORM, giving you simple-to-use repositories for all your database access code.

## Installation:

I recommend using the NuGet package: https://www.nuget.org/packages/Dapper.Repository/ however you can also simply clone the repository and compile the project yourself.

As the project is licensed under MIT you're free to use it for pretty much anything you want.

You also need to install Dapper yourself, again I'd recommend NuGet: https://www.nuget.org/packages/Dapper/

As for versioning of Dapper, you're actually (somewhat) free to choose whichever you want, as this library isn't built targetting a specific version of Dapper. 
Instead whatever Dapper version you prefer is injected into this extension library. This leaves you free to update Dapper without waiting for a new version of this library.

## Requirements:

The library requires .Net 5.0 with C# 9 or later, as it's using the "record" type rather than "class" for representing the single entities. (e.g. An "User" from your "Users" table)

Also it currently only supports MS Sql and MySql, but feel free to branch it and create support for PostGre or whatever you're using (as long as Dapper supports it, this library can too)

## Upcoming features:

- Repository for aggregates, like e.g. an Order entity with an IList\<OrderLine\> property containing all the orderlines for the order. The idea is you can just call any CRUD method on the repository with an aggregate, and it'll figure out foreign keys etc. for you to ensure everything is inserted/updated/deleted/retrieved in the proper order. Kind of like what Entity Framework does, just better :-P
- Built-in caching with automatic cache invalidation

## Namespaces:

I've gone with the same class names for both the Ms Sql and MySql base classes, the only difference being their namespace.
So if you're using Ms Sql use `Dapper.Repository.Sql`, and for MySql use `Dapper.Repository.MySql`.

## Usage:

In order to avoid building this library for a specific Dapper version, I've added an injection point for injecting the necessary Dapper extension methods into the repositories.  
To only do this once, I recommend you start by creating a class called DapperInjection:

    using Dapper.Repository;
    using SqlMapper = Dapper.SqlMapper;
    
    namespace YourNameSpaceHere
    {
        internal class DapperInjection<TEntity> : IDapperInjection<TEntity>
        {
            public QuerySingleDelegate<TEntity> QuerySingle => SqlMapper.QuerySingle<TEntity>;

            public QuerySingleDelegate<TEntity> QuerySingleOrDefault => SqlMapper.QuerySingleOrDefault<TEntity>;

            public QueryDelegate<TEntity> Query => SqlMapper.Query<TEntity>;

            public QuerySingleAsyncDelegate<TEntity> QuerySingleAsync => SqlMapper.QuerySingleAsync<TEntity>;

            public QuerySingleAsyncDelegate<TEntity> QuerySingleOrDefaultAsync => SqlMapper.QuerySingleOrDefaultAsync<TEntity>;

            public QueryAsyncDelegate<TEntity> QueryAsync => SqlMapper.QueryAsync<TEntity>;

            public ExecuteDelegate Execute => SqlMapper.Execute;

            public ExecuteAsyncDelegate ExecuteAsync => SqlMapper.ExecuteAsync;
        }
    }

What's going on here is you're injecting delegates to the Dapper extension methods into the Repository library.

Secondly I'd recommend creating a sort of "base repository" class for your project, which handles creating an IDbConnection etc. It could look something like this:

    using System.Data;
    using System.Data.SqlClient;
    using Dapper.Repository.Sql;

    namespace YourNameSpaceHere
    {
        public abstract class BasePrimaryKeyRepository<TPrimaryKeyEntity, TEntity> : PrimaryKeyRepository<TPrimaryKeyEntity, TEntity>
        where TPrimaryKeyEntity : DbEntity
        where TEntity : TPrimaryKeyEntity
        {
            protected override IDbConnection CreateConnection()
            {
                return new SqlConnection("Your connection string"); // You probably shouldn't hardcode your connection strings, this is just an example
            }

            protected override IDapperInjection<T> CreateDapperInjection<T>()
            {
                return new DapperInjection<T>();
            }
        }
    }

The "base repository" we just created is for tables with a primary key. If you have any heap tables (tables without a primary key), you should create a similar "BaseHeapRepository" class inheriting HeapRepository.


That's the prerequisites taken care of, now onto actually using these classes. For this example we're going to create a very basic UserRepository mapping to a "Users" table looking like this:

    CREATE TABLE Users
    (
        UserID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Username VARCHAR(50) NOT NULL,
        Password VARCHAR(50) NOT NULL, // Don't store passwords in plain text please, this is just for illustration purposes
        Description VARCHAR(MAX) NULL,
        DateCreated DATETIME() NOT NULL DEFAULT(GETDATE())
    )


Now in order to have some nice method overloads, we're actually splitting the entity class into two, using inheritance along the way. We're also using the type "record" instead of "class". This is because it simplifies making entities immutable and creating new instances with the changes you want. Immutability allows for some very aggressive memory caching, because the entities become (at least somewhat) thread-safe.

Our UserEntity.cs file would therefore look like this:

    using System;
    using Dapper.Repository.Attributes;

    namespace YourNameSpaceHere
    {
        public record UserPrimaryKeyEntity : DbEntity
        {
            [PrimaryKeyColumn(isIdentity: true)]
            public int Id { get; init; }
        }

        public record UserEntity : UserPrimaryKeyEntity
        {
            [Column]
            public string Username { get; init; } = default!;

            [Column]
            public string Password { get; init; } = default!;

            [Column]
            public string? Description { get; init; }

            [Column(hasDefaultConstraint: true)] // Marked with "hasDefaultConstraint", as the table does indeed have DEFAULT(GETDATE()) on this column
            public DateTime DateCreated { get; } // No init as I want this property completely read-only in .Net, it's only ever set once by the SQL server
        }
    }

You'll notice I've enabled nullable in the project in this case, and I'm marking Description as string?. If you're not working with nullable just remove the ?, and the " = default!;" too.  
The " = default!;" part is basically here to suppress warning *CS8616: Non-nullable property 'Username' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.*  
What this does, is it sets the property to null, and adds the "null-forgiving operator !" to tell the compiler to stop complaining.

This is ok here, because we're either getting entities from the SQL server, in which case the properties won't be null, or creating a full instance to insert, in which case it also won't be null.


Why the split between two record types you ask? It's for simplifying the Delete and Get methods on the repository.

Rather than having to supply a full UserEntity instance as parameter to them, you can now just supply the UserPrimaryKeyEntity (e.g. the Id of the user to Delete or Get)

The final part is defining your repository, this in turn requires very little code as most of the functionality is built-in:

    namespace YourNameSpaceHere
    {
        public class UserRepository : BasePrimaryKeyRepository<UserPrimaryKeyEntity, UserEntity>
        {
            protected override string TableName => "Users";
        }
    }

Quite easy that one huh? :-)

The UserRepository now gives you access to the following built-in methods:
- UserEntity Delete(UserPrimaryKeyEntity entity)
- UserEntity Insert(UserEntity entity)
- UserEntity Get(UserPrimaryKeyEntity entity)
- IEnumerable<UserEntity> GetAll()
- UserEntity Update(UserEntity entity)

All of which have an Async variant as well. You'll notice all methods, including Delete, return an instance of UserEntity. This is because whatever data you've just manipulated using Delete, Insert or Update is returned to you as an instance.  
In our case this means the result from Insert will actually contain the DateCreated value the database generated itself.  
Furthermore for deletions you're getting all the properties from the record you've just deleted, which can be quite handy for cache invalidation.

Should you want to add custom queries to your repository, it has a bunch of the Dapper extensions built-in for you to call, they create a connection themselves so it's as simple as:

    public IEnumerable<UserEntity> GetUsersWithoutDescription()
    {
        return Query($"SELECT * FROM {FormattedTableName} WHERE Description IS NULL");
    }

Notice the {FormattedTableName} there, that's a read-only property which contains the proper Schema and TableName correctly formatted. By using this rather than typing out the name yourself, it'll be easier for you if you ever rename the table or move it to a different Schema.


Furthermore the PrimaryKey based repository has these public (non-static!) events:
- PreDelete
- PostDelete
- PreInsert
- PostInsert
- PreUpdate
- PostUpdate

The Heap based repository has the same events except for the Update ones, as there's no Update method here.


The Pre- events all contain the entity you've passed as an argument to the respective method, as well as an CancelableEventArgs allowing any event handler to cancel the operation in question.
If any listener cancels the event, an CanceledException is thrown by the method in question.

The Post- events all contain the entity returned from the database, so for an insert any default constraint, identity columns etc. will have their values fresh from the DB.

Finally all the events SWALLOW EXCEPTIONS, so any exception handling you want for custom code in an event handler you'll have to implement directly in your handler. This is to ensure no third party code breaks the DB operation.


One finally notice: I'd highly recommend checking out the different parameters you can assign to [Column] and [PrimaryKey], as well as checking out what properties can be overridden in your BasePrimaryKeyRepository and which methods it already has for you to use. This should give you further insight into how the library works.
