using Dapper.Repository.Repositories;
using Dapper.Repository.UnitTests.Aggregates;

namespace Dapper.Repository.UnitTests.Repositories;
public class TableRepositoryTests
{

	[Fact]
	public void Test_NoConnectionFactory_Throws()
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
	public void Test_NoQueryGeneratorFactory_Throws()
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
	public void Test_NoDapperInjectionFactory_Throws()
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
}
