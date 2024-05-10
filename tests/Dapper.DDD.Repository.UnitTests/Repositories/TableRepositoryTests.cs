using Dapper.DDD.Repository.Repositories;
using Dapper.DDD.Repository.UnitTests.Aggregates;

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
}