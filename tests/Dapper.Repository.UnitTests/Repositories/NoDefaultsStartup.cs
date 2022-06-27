using Dapper.Repository.DependencyInjection;
using Dapper.Repository.UnitTests.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Repository.UnitTests.Repositories;

public class NoDefaultsStartup
{
	public NoDefaultsStartup()
	{
		var services = new ServiceCollection();
		_ = services.AddOptions();
		_ = services.AddTableRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.TableName = "Users";
			options.HasKey(x => x.Id);
		});
		_ = services.AddViewRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = Mock.Of<IQueryGeneratorFactory>();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.ViewName = "Users";
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
