using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Repositories;

public class NoDefaultsStartup
{
	public NoDefaultsStartup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.AddTableRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.TableName = "Users";
			options.HasKey(x => x.Id);
		});
		services.AddViewRepository<UserAggregate, Guid>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.ViewName = "Users";
			options.HasKey(x => x.Id);
		});
		services.AddTableRepository<StatefulAggregate, Guid, IStatefulTableRepository, StatefulTableRepository>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.TableName = "Stateful";
			options.HasKey(x => x.Id);
		}, (options, defaultOptions, provider) =>
		{
			return new StatefulTableRepository(options, defaultOptions) { State = Guid.NewGuid() };
		});
		services.AddViewRepository<StatefulAggregate, Guid, IStatefulViewRepository, StatefulViewRepository>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.ViewName = "Stateful";
			options.HasKey(x => x.Id);
		}, (options, defaultOptions, provider) =>
		{
			return new StatefulViewRepository(options, defaultOptions) { State = Guid.NewGuid() };
		});
		services.AddViewRepository<StatefulAggregate, IStatefulSimpleViewRepository, StatefulSimpleViewRepository>(options =>
		{
			options.ConnectionFactory = Mock.Of<IConnectionFactory>();
			options.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			options.DapperInjectionFactory = Mock.Of<IDapperInjectionFactory>();
			options.ViewName = "Stateful";
		}, (options, defaultOptions, provider) =>
		{
			return new StatefulSimpleViewRepository(options, defaultOptions) { State = Guid.NewGuid() };
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}