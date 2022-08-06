using Dapper.DDD.Repository.Repositories;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Repositories;
public class ViewRepositoryTests : IClassFixture<NoDefaultsStartup>
{
	private ServiceProvider _provider;

	public ViewRepositoryTests(NoDefaultsStartup startup)
	{
		_provider = startup.Provider;
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
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory()
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
		new ViewRepository<UserAggregate, Guid>(Options.Create(
			config
		), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("ConnectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoQueryGeneratorFactory_Throws()
	{
		// Arrange 
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			ConnectionFactory = Mock.Of<IConnectionFactory>()
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					config
				), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("QueryGeneratorFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoDapperInjectionFactory_Throws()
	{
		// Arrange
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory()
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					config
				), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("DapperInjectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoViewName_Throws()
	{
		// Arrange
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			Schema = "dbo"
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					config
				), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("ViewName", ex.Message);
	}

	[Fact]
	public void Constructor_NoKey_Throws()
	{
		// Arrange
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			ViewName = "Users"
		};

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					config
				), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("No key has been specified for this aggregate", ex.Message);
	}

	[Fact]
	public void ConstructorViewWithoutId_NoKey_Valid()
	{
		// Arrange
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			ViewName = "Users"
		};

		// Act
		var repo = new ViewRepository<UserAggregate>(Options.Create(
			config
		), Options.Create(new Configuration.DefaultConfiguration()));

		// Assert
		Assert.NotNull(repo);
	}

	[Fact]
	public void Constructor_NoSchemaName_Valid()
	{
		// Arrange
		var config = new Configuration.ViewAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = new MockQueryGeneratorFactory(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			ViewName = "Users"
		};
		config.HasKey(x => x.Id);

		var repo = new ViewRepository<UserAggregate, Guid>(Options.Create(
			config
		), Options.Create(new Configuration.DefaultConfiguration()));

		// Assert
		Assert.NotNull(repo);
	}
}
