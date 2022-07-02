using Dapper.Repository.DependencyInjection;
using Dapper.Repository.UnitTests.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Repository.UnitTests.Repositories;

public class NoDefaultsStartup
{
	public NoDefaultsStartup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.AddTableRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.TableName = "Users";
			options.HasKey(x => x.Id);
		});
		services.AddViewRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.ViewName = "Users";
			options.HasKey(x => x.Id);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
