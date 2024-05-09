using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Interfaces;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.DependencyInjection;

public delegate TRepository TableRepositoryConstructorDelegate<TAggregate, out TRepository>(
	IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions, IServiceProvider provider)
	where TAggregate : notnull;

public delegate TRepository ViewRepositoryConstructorDelegate<TAggregate, out TRepository>(
	IOptions<ViewAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions,
	IServiceProvider provider)
	where TAggregate : notnull;

public static class DapperRepositoryDependencyInjection
{
	/// <summary>
	///     Configure defaults to use for all aggregate types.
	/// </summary>
	public static IServiceCollection ConfigureDapperRepositoryDefaults(this IServiceCollection services,
		Action<DefaultConfiguration> configureOptions)
	{
		return services.Configure(configureOptions);
	}
	
	/// <summary>
	///     Add a table repository for the given aggregate type to the dependency injection system.
	///     You can request an ITableRepository<TAggregate, TAggregateId> through the dependency injection system afterwards.
	/// </summary>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId>(this IServiceCollection services,
		Action<TableAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
		where TAggregateId : notnull
	{
		services.Configure(configureOptions);
		services
			.AddSingleton<ITableRepository<TAggregate, TAggregateId>, TableRepository<TAggregate, TAggregateId>>();
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     You can request an IViewRepository<TAggregate, TAggregateId> through the dependency injection system afterwards.
	/// </summary>
	public static IServiceCollection AddViewRepository<TAggregate, TAggregateId>(this IServiceCollection services,
		Action<ViewAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
		where TAggregateId : notnull
	{
		services.Configure(configureOptions);
		services
			.AddSingleton<IViewRepository<TAggregate, TAggregateId>, ViewRepository<TAggregate, TAggregateId>>();
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     You can request an IViewRepository<TAggregate> through the dependency injection system afterwards.
	/// </summary>
	public static IServiceCollection AddViewRepository<TAggregate>(this IServiceCollection services,
		Action<ViewAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
	{
		services.Configure(configureOptions);
		services.AddSingleton<IViewRepository<TAggregate>, ViewRepository<TAggregate>>();
		return services;
	}
	
	/// <summary>
	///     Add a table repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in TableRepository type.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TAggregateId">Type of your aggregate's Id</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId, TRepositoryInterface,
		TRepositoryClass>(this IServiceCollection services,
		Action<TableAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
		where TAggregateId : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : TableRepository<TAggregate, TAggregateId>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface, TRepositoryClass>();
		return services;
	}
	
	
	/// <summary>
	///     Add a table repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in TableRepository type.
	///     Takes a custom function to create the actual instance, allowing you to set any instance-specific state.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TAggregateId">Type of your aggregate's Id</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId, TRepositoryInterface,
		TRepositoryClass>(this IServiceCollection services,
		Action<TableAggregateConfiguration<TAggregate>> configureOptions,
		TableRepositoryConstructorDelegate<TAggregate, TRepositoryClass> constructor)
		where TAggregate : notnull
		where TAggregateId : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : TableRepository<TAggregate, TAggregateId>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface>(provider =>
		{
			var configuration = provider.GetRequiredService<IOptions<TableAggregateConfiguration<TAggregate>>>();
			var defaultConfiguration = provider.GetService<IOptions<DefaultConfiguration>>();
			return constructor(configuration, defaultConfiguration, provider);
		});
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in ViewRepository type.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TAggregateId">Type of your aggregate's Id</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddViewRepository<TAggregate, TAggregateId, TRepositoryInterface,
		TRepositoryClass>(this IServiceCollection services,
		Action<ViewAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
		where TAggregateId : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : ViewRepository<TAggregate, TAggregateId>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface, TRepositoryClass>();
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in ViewRepository type.
	///     Takes a custom function to create the actual instance, allowing you to set any instance-specific state.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TAggregateId">Type of your aggregate's Id</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddViewRepository<TAggregate, TAggregateId, TRepositoryInterface,
		TRepositoryClass>(this IServiceCollection services,
		Action<ViewAggregateConfiguration<TAggregate>> configureOptions,
		ViewRepositoryConstructorDelegate<TAggregate, TRepositoryClass> constructor)
		where TAggregate : notnull
		where TAggregateId : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : ViewRepository<TAggregate, TAggregateId>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface>(provider =>
		{
			var configuration = provider.GetRequiredService<IOptions<ViewAggregateConfiguration<TAggregate>>>();
			var defaultConfiguration = provider.GetService<IOptions<DefaultConfiguration>>();
			return constructor(configuration, defaultConfiguration, provider);
		});
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in ViewRepository type.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddViewRepository<TAggregate, TRepositoryInterface, TRepositoryClass>(
		this IServiceCollection services, Action<ViewAggregateConfiguration<TAggregate>> configureOptions)
		where TAggregate : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : ViewRepository<TAggregate>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface, TRepositoryClass>();
		return services;
	}
	
	/// <summary>
	///     Add a view repository for the given aggregate type to the dependency injection system.
	///     Uses a custom class and interface, allowing you to inherit from the built-in ViewRepository type.
	///     Takes a custom function to create the actual instance, allowing you to set any instance-specific state.
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddViewRepository<TAggregate, TRepositoryInterface, TRepositoryClass>(
		this IServiceCollection services, Action<ViewAggregateConfiguration<TAggregate>> configureOptions,
		ViewRepositoryConstructorDelegate<TAggregate, TRepositoryClass> constructor)
		where TAggregate : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : ViewRepository<TAggregate>, TRepositoryInterface
	{
		services.Configure(configureOptions);
		services.AddSingleton<TRepositoryInterface>(provider =>
		{
			var configuration = provider.GetRequiredService<IOptions<ViewAggregateConfiguration<TAggregate>>>();
			var defaultConfiguration = provider.GetService<IOptions<DefaultConfiguration>>();
			return constructor(configuration, defaultConfiguration, provider);
		});
		return services;
	}
}