using Dapper.Repository.Configuration;
using Dapper.Repository.Interfaces;
using Dapper.Repository.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class DapperRepositoryDependencyInjection
{
	public static IServiceCollection ConfigureDapperRepositoryDefaults(this IServiceCollection services, Action<DefaultConfiguration> configureOptions)
	{
		return services.Configure(configureOptions);
	}

	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId>(this IServiceCollection services, Action<IAggregateConfiguration<TAggregate>> configureOptions)
	where TAggregate : notnull
	{
		services.Configure(configureOptions);
		services.AddSingleton<TableRepository<TAggregate, TAggregateId>>();
		return services;
	}
}