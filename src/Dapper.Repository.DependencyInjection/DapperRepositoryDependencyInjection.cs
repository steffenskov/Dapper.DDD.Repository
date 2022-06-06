using Dapper.Repository.Configuration;
using Dapper.Repository.Interfaces;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Repository.DependencyInjection;

public static class DapperRepositoryDependencyInjection
{
	/// <summary>
	/// Configure defaults to use for all aggregate types.
	/// </summary>
	public static IServiceCollection ConfigureDapperRepositoryDefaults(this IServiceCollection services, Action<DefaultConfiguration> configureOptions)
	{
		return services.Configure(configureOptions);
	}

	/// <summary>
	/// Add a table repository for the given aggregate type to the dependency injection system.
	/// You can inject an ITableRepository<TAggregate, TAggregateId> into your own repositories afterwards.
	/// </summary>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId>(this IServiceCollection services, Action<TableAggregateConfiguration<TAggregate>> configureOptions)
	where TAggregate : notnull
	where TAggregateId : notnull
	{
		_ = services.Configure(configureOptions);
		_ = services.AddSingleton<ITableRepository<TAggregate, TAggregateId>, TableRepository<TAggregate, TAggregateId>>();
		return services;
	}

	/// <summary>
	/// Add a view repository for the given aggregate type to the dependency injection system.
	/// You can inject an IViewRepository<TAggregate, TAggregateId> into your own repositories afterwards.
	/// </summary>
	public static IServiceCollection AddViewRepository<TAggregate, TAggregateId>(this IServiceCollection services, Action<ViewAggregateConfiguration<TAggregate>> configureOptions)
	where TAggregate : notnull
	where TAggregateId : notnull
	{
		_ = services.Configure(configureOptions);
		_ = services.AddSingleton<IViewRepository<TAggregate, TAggregateId>, ViewRepository<TAggregate, TAggregateId>>();
		return services;
	}
}