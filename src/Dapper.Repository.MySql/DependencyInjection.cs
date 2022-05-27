using Dapper.Repository.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class DapperRepositoryDependencyInjection
{
	/// <summary>
	/// Configure defaults to use for all aggregate types.
	/// </summary>
	public static IServiceCollection ConfigureDapperRepositoryDefaults(this IServiceCollection services, Action<MySqlDefaultConfiguration> configureOptions)
	{
		return services.Configure(configureOptions);
	}

	/// <summary>
	/// Add a table repository for the given aggregate type to the dependency injection system.
	/// You can inject an ITableRepository<TAggregate, TAggregateId> into your own repositories afterwards.
	/// </summary>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId>(this IServiceCollection services, Action<MySqlAggregateConfiguration<TAggregate>> configureOptions)
	where TAggregate : notnull
	where TAggregateId : notnull
	{
		services.Configure(configureOptions);
		services.AddSingleton<ITableRepository<TAggregate, TAggregateId>, TableRepository<TAggregate, TAggregateId>>();
		return services;
	}

	/// <summary>
	/// Add a view repository for the given aggregate type to the dependency injection system.
	/// You can inject an IViewRepository<TAggregate, TAggregateId> into your own repositories afterwards.
	/// </summary>
	public static IServiceCollection AddViewRepository<TAggregate, TAggregateId>(this IServiceCollection services, Action<MySqlAggregateConfiguration<TAggregate>> configureOptions)
	where TAggregate : notnull
	where TAggregateId : notnull
	{
		services.Configure(configureOptions);
		services.AddSingleton<ITableRepository<TAggregate, TAggregateId>, TableRepository<TAggregate, TAggregateId>>(); // TODO: Implement ViewRepository AND a ViewAggregateConfiguration as TableName becomes wrong
		return services;
	}
}