using System.Data;
using Dapper.DDD.Repository.Repositories;
using Dapper.DDD.Repository.Sql;
using Dapper.DDD.Repository.UnitTests.Aggregates;
using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Repositories;

public class TableRepositoryTests : IClassFixture<NoDefaultsStartup>
{
	private readonly ServiceProvider _provider;

	public TableRepositoryTests(NoDefaultsStartup startup)
	{
		_provider = startup.Provider;
	}

	[Fact]
	public void StatefulRepository_HasState_CanBeSeen()
	{
		// Arrange
		var repo = _provider.GetRequiredService<IStatefulTableRepository>();

		// Assert
		Assert.NotEqual(Guid.Empty, repo.State);
	}

	[Fact]
	public void DependencyInjection_NoDefaultsConfiguration_StillWorks()
	{
		// Act
		var repo = _provider.GetService<ITableRepository<UserAggregate, Guid>>();

		// Assert
		Assert.NotNull(repo);
	}

	[Fact]
	public void Constructor_NoConnectionFactory_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new TableRepository<UserAggregate, Guid>(Options.Create(
				new TableAggregateConfiguration<UserAggregate>
				{
					DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
					QueryGeneratorFactory = new MockQueryGeneratorFactory()
				}
			), Options.Create(new DefaultConfiguration())));

		Assert.Contains("ConnectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoQueryGeneratorFactory_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new TableRepository<UserAggregate, Guid>(Options.Create(
				new TableAggregateConfiguration<UserAggregate>
				{
					DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
					ConnectionFactory = Substitute.For<IConnectionFactory>()
				}
			), Options.Create(new DefaultConfiguration())));

		Assert.Contains("QueryGeneratorFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoDapperInjectionFactory_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new TableRepository<UserAggregate, Guid>(Options.Create(
				new TableAggregateConfiguration<UserAggregate>
				{
					ConnectionFactory = Substitute.For<IConnectionFactory>(),
					QueryGeneratorFactory = new MockQueryGeneratorFactory()
				}
			), Options.Create(new DefaultConfiguration())));

		Assert.Contains("DapperInjectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoTableName_Throws()
	{
		// Arrange
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			Schema = "dbo"
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new TableRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));

		Assert.Contains("TableName", ex.Message);
	}

	[Fact]
	public void Constructor_NoKey_Throws()
	{
		// Arrange
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			TableName = "Users"
		};

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() =>
			new TableRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));

		Assert.Contains("No key has been specified for this aggregate", ex.Message);
	}

	[Fact]
	public void Constructor_NoSchemaName_Valid()
	{
		// Arrange
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		// Act
		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(
			config
		), Options.Create(new DefaultConfiguration()));

		// Assert
		Assert.NotNull(repo);
	}

	#region Trigger handling

	[Fact]
	public async Task DeleteAsync_DoesNotHaveTriggers_Queries()
	{
		// Arrange
		var executed = false;
		var queried = false;
		var query = "";
		var dapperInjection = Substitute.For<IDapperInjection<UserAggregate>>();
		dapperInjection.When(injection => injection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			executed = true;
			query = callInfo.ArgAt<string>(1);
		});
		dapperInjection.When(injection => injection.QuerySingleOrDefaultAsync(Arg.Any<IDbConnection>(), Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			queried = true;
			query = callInfo.ArgAt<string>(2);
		});
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		dapperInjectionFactory.Create<UserAggregate>().Returns(dapperInjection);
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		var result = await repo.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.False(executed);
		Assert.True(queried);
		Assert.Null(result);
		Assert.NotNull(query);
		Assert.Contains("DELETE", query);
		Assert.Contains("OUTPUT", query);
	}

	[Fact]
	public async Task DeleteAsync_HasTriggers_ExecutesInsteadOfQuerying()
	{
		// Arrange
		var executed = false;
		var queried = false;
		var query = "";
		var dapperInjection = Substitute.For<IDapperInjection<UserAggregate>>();
		dapperInjection.When(injection => injection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			executed = true;
			query = callInfo.ArgAt<string>(1);
		});
		dapperInjection.When(injection => injection.QuerySingleOrDefaultAsync(Arg.Any<IDbConnection>(), Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			queried = true;
			query = callInfo.ArgAt<string>(2);
		});
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		dapperInjectionFactory.Create<UserAggregate>().Returns(dapperInjection);
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			HasTriggers = true,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		var result = await repo.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.True(executed);
		Assert.False(queried);
		Assert.Null(result);
		Assert.NotNull(query);
		Assert.Contains("DELETE", query);
		Assert.DoesNotContain("OUTPUT", query);
	}

	[Fact]
	public async Task UpdateAsync_DoesNotHaveTriggers_Queries()
	{
		// Arrange
		var executed = false;
		var queried = false;
		var query = "";
		var dapperInjection = Substitute.For<IDapperInjection<UserAggregate>>();
		dapperInjection.When(injection => injection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			executed = true;
			query = callInfo.ArgAt<string>(1);
		});
		dapperInjection.When(injection => injection.QuerySingleOrDefaultAsync(Arg.Any<IDbConnection>(), Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			queried = true;
			query = callInfo.ArgAt<string>(2);
		});
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		dapperInjectionFactory.Create<UserAggregate>().Returns(dapperInjection);
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		var result = await repo.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.False(executed);
		Assert.True(queried);
		Assert.Null(result);
		Assert.NotNull(query);
		Assert.Contains("DELETE", query);
		Assert.Contains("OUTPUT", query);
	}

	[Fact]
	public async Task UpdateAsync_HasTriggers_ExecutesInsteadOfQuerying()
	{
		// Arrange
		var executed = false;
		var queried = false;
		var query = "";
		var dapperInjection = Substitute.For<IDapperInjection<UserAggregate>>();
		dapperInjection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>()).Returns(1);
		dapperInjection.When(injection => injection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			executed = true;
			query = callInfo.ArgAt<string>(1);
		});
		dapperInjection.When(injection => injection.QuerySingleOrDefaultAsync(Arg.Any<IDbConnection>(), Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			queried = true;
			query = callInfo.ArgAt<string>(2);
		});
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		dapperInjectionFactory.Create<UserAggregate>().Returns(dapperInjection);
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			HasTriggers = true,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		var result = await repo.UpdateAsync(new UserAggregate
		{
			DeliveryAddress = new Address("Street", "City"),
			Id = Guid.NewGuid(),
			InvoiceAddress = new Address("Street", "City")
		});

		// Assert
		Assert.True(executed);
		Assert.False(queried);
		Assert.NotNull(result); // Since this doesn't exist in the db
		Assert.NotNull(query);
		Assert.Contains("UPDATE", query);
		Assert.DoesNotContain("OUTPUT", query);
	}

	[Fact]
	public async Task UpdateAsync_HasTriggersButRowDoesNotExist_ExecutesInsteadOfQuerying()
	{
		// Arrange
		var executed = false;
		var queried = false;
		var query = "";
		var dapperInjection = Substitute.For<IDapperInjection<UserAggregate>>();
		dapperInjection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>()).Returns(0);
		dapperInjection.When(injection => injection.ExecuteAsync(Arg.Any<IDbConnection>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			executed = true;
			query = callInfo.ArgAt<string>(1);
		});
		dapperInjection.When(injection => injection.QuerySingleOrDefaultAsync(Arg.Any<IDbConnection>(), Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())).Do(callInfo =>
		{
			queried = true;
			query = callInfo.ArgAt<string>(2);
		});
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		dapperInjectionFactory.Create<UserAggregate>().Returns(dapperInjection);
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			HasTriggers = true,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		var result = await repo.UpdateAsync(new UserAggregate
		{
			DeliveryAddress = new Address("Street", "City"),
			Id = Guid.NewGuid(),
			InvoiceAddress = new Address("Street", "City")
		});

		// Assert
		Assert.True(executed);
		Assert.False(queried);
		Assert.Null(result); // Since this doesn't exist in the db
		Assert.NotNull(query);
		Assert.Contains("UPDATE", query);
		Assert.DoesNotContain("OUTPUT", query);
	}

	[Fact]
	public async Task UpsertAsync_HasTriggers_Throws()
	{
		// Arrange
		var dapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
		var config = new TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory(),
			DapperInjectionFactory = dapperInjectionFactory,
			HasTriggers = true,
			Schema = "dbo",
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(config), Options.Create(new DefaultConfiguration()));

		// Act
		await Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.UpsertAsync(new UserAggregate
		{
			DeliveryAddress = new Address("Street", "City"),
			Id = Guid.NewGuid(),
			InvoiceAddress = new Address("Street", "City")
		}));
	}

	#endregion
}