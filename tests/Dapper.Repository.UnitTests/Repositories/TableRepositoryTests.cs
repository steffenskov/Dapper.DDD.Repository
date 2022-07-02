using Dapper.Repository.Repositories;
using Dapper.Repository.UnitTests.Aggregates;

namespace Dapper.Repository.UnitTests.Repositories;
public class TableRepositoryTests : IClassFixture<NoDefaultsStartup>
{
	private ServiceProvider _provider;

	public TableRepositoryTests(NoDefaultsStartup startup)
	{
		_provider = startup.Provider;
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
			new Configuration.TableAggregateConfiguration<UserAggregate>
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
				new TableRepository<UserAggregate, Guid>(Options.Create(
					new Configuration.TableAggregateConfiguration<UserAggregate>
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
				new TableRepository<UserAggregate, Guid>(Options.Create(
					new Configuration.TableAggregateConfiguration<UserAggregate>
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
	public void Constructor_NoTableName_Throws()
	{
		// Arrange
		var config = new Configuration.TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			Schema = "dbo"
		};
		config.HasKey(x => x.Id);

		// Act && Assert
		var ex = Assert.Throws<ArgumentNullException>(() =>
				new TableRepository<UserAggregate, Guid>(Options.Create(
					config
				), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("TableName", ex.Message);
	}

	[Fact]
	public void Constructor_NoKey_Throws()
	{
		// Arrange
		var config = new Configuration.TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			TableName = "Users"
		};

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() =>
						new TableRepository<UserAggregate, Guid>(Options.Create(
							config
						), Options.Create(new Configuration.DefaultConfiguration())));

		Assert.Contains("No key has been specified for this aggregate", ex.Message);
	}

	[Fact]
	public void Constructor_NoSchemaName_Valid()
	{
		// Arrange
		var config = new Configuration.TableAggregateConfiguration<UserAggregate>
		{
			ConnectionFactory = Mock.Of<IConnectionFactory>(),
			QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>(),
			DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>(),
			TableName = "Users"
		};
		config.HasKey(x => x.Id);

		// Act
		var repo = new TableRepository<UserAggregate, Guid>(Options.Create(
			config
		), Options.Create(new Configuration.DefaultConfiguration()));

		// Assert
		Assert.NotNull(repo);
	}
}
