using Dapper.DDD.Repository.Repositories;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Repositories;

public class ViewRepositoryTests : IClassFixture<NoDefaultsStartup>
{
	private readonly ServiceProvider _provider;
	
	public ViewRepositoryTests(NoDefaultsStartup startup)
	{
		_provider = startup.Provider;
	}
	
	[Fact]
	public void StatefulRepositories_HasState_StatesAreDifferent()
	{
		// Arrange
		var repo1 = _provider.GetRequiredService<IStatefulViewRepository>();
		var repo2 = _provider.GetRequiredService<IStatefulSimpleViewRepository>();
		
		// Assert
		Assert.NotEqual(Guid.Empty, repo1.State);
		Assert.NotEqual(Guid.Empty, repo2.State);
		Assert.NotEqual(repo1.State, repo2.State);
	}
	
	[Fact]
	public void DependencyInjection_NoDefaultsConfiguration_StillWorks()
	{
		// Act
		var repo = _provider.GetService<IViewRepository<UserAggregate, Guid>>();
		
		// Assert
		Assert.NotNull(repo);
	}
	
	[Fact]
	public void Constructor_NoConnectionFactory_Throws()
	{
		// Arrange 
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory()
		};
		config.HasKey(x => x.Id);
		
		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new ViewRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));
		
		Assert.Contains("ConnectionFactory", ex.Message);
	}
	
	[Fact]
	public void Constructor_NoQueryGeneratorFactory_Throws()
	{
		// Arrange 
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			ConnectionFactory = Substitute.For<IConnectionFactory>()
		};
		config.HasKey(x => x.Id);
		
		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new ViewRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));
		
		Assert.Contains("QueryGeneratorFactory", ex.Message);
	}
	
	[Fact]
	public void Constructor_NoDapperInjectionFactory_Throws()
	{
		// Arrange
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory()
		};
		config.HasKey(x => x.Id);
		
		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new ViewRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));
		
		Assert.Contains("DapperInjectionFactory", ex.Message);
	}
	
	[Fact]
	public void Constructor_NoViewName_Throws()
	{
		// Arrange
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			Schema = "dbo"
		};
		config.HasKey(x => x.Id);
		
		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
			new ViewRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));
		
		Assert.Contains("ViewName", ex.Message);
	}
	
	[Fact]
	public void Constructor_NoKey_Throws()
	{
		// Arrange
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			ViewName = "Users"
		};
		
		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() =>
			new ViewRepository<UserAggregate, Guid>(Options.Create(
				config
			), Options.Create(new DefaultConfiguration())));
		
		Assert.Contains("No key has been specified for this aggregate", ex.Message);
	}
	
	[Fact]
	public void ConstructorViewWithoutId_NoKey_Valid()
	{
		// Arrange
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			ViewName = "Users"
		};
		
		// Act
		var repo = new ViewRepository<UserAggregate>(Options.Create(
			config
		), Options.Create(new DefaultConfiguration()));
		
		// Assert
		Assert.NotNull(repo);
	}
	
	[Fact]
	public void Constructor_NoSchemaName_Valid()
	{
		// Arrange
		var config = new ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Substitute.For<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>(),
			ViewName = "Users"
		};
		config.HasKey(x => x.Id);
		
		var repo = new ViewRepository<UserAggregate, Guid>(Options.Create(
			config
		), Options.Create(new DefaultConfiguration()));
		
		// Assert
		Assert.NotNull(repo);
	}
}