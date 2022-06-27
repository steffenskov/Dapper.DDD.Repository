using Dapper.Repository.Repositories;
using Dapper.Repository.UnitTests.Aggregates;

namespace Dapper.Repository.UnitTests.Repositories;
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
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
		new ViewRepository<UserAggregate, Guid>(Options.Create(
			new Configuration.ViewAggregateConfiguration<UserAggregate>
			{
				DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
				QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>()

			}
		), Options.Create(new Configuration.DefaultConfiguration
		{

		})));

		Assert.Contains("ConnectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoQueryGeneratorFactory_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					new Configuration.ViewAggregateConfiguration<UserAggregate>
					{
						DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
						ConnectionFactory = Mock.Of<IConnectionFactory>()

					}
				), Options.Create(new Configuration.DefaultConfiguration
				{

				})));

		Assert.Contains("QueryGeneratorFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoDapperInjectionFactory_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					new Configuration.ViewAggregateConfiguration<UserAggregate>
					{
						ConnectionFactory = Mock.Of<IConnectionFactory>(),
						QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>()

					}
				), Options.Create(new Configuration.DefaultConfiguration
				{

				})));

		Assert.Contains("DapperInjectionFactory", ex.Message);
	}

	[Fact]
	public void Constructor_NoViewName_Throws()
	{
		// Arrange && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new ViewRepository<UserAggregate, Guid>(Options.Create(
					new Configuration.ViewAggregateConfiguration<UserAggregate>
					{
						ConnectionFactory = Mock.Of<IConnectionFactory>(),
						QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>(),
						DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
						Schema = "dbo"
					}
				), Options.Create(new Configuration.DefaultConfiguration
				{

				})));

		Assert.Contains("ViewName", ex.Message);
	}

	[Fact]
	public void Constructor_NoSchemaName_Valid()
	{
		// Arrange
		var repo = new ViewRepository<UserAggregate, Guid>(Options.Create(
			new Configuration.ViewAggregateConfiguration<UserAggregate>
			{
				ConnectionFactory = Mock.Of<IConnectionFactory>(),
				QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>(),
				DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
				ViewName = "Users"
			}
		), Options.Create(new Configuration.DefaultConfiguration
		{

		}));

		// Assert
		Assert.NotNull(repo);
	}
}
