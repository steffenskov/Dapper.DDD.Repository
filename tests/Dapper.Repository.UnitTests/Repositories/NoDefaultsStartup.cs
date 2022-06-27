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
			options.TableName = "Users";
			options.HasKey(x => x.Id);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
