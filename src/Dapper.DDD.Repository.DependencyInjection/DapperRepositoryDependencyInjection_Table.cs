namespace Dapper.DDD.Repository.DependencyInjection;

public delegate TRepository TableRepositoryConstructorDelegate<TAggregate, out TRepository>(
	IOptions<TableAggregateConfiguration<TAggregate>> options, IOptions<DefaultConfiguration>? defaultOptions, IServiceProvider provider)
	where TAggregate : notnull;

public static partial class DapperRepositoryDependencyInjection
{
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
	/// </summary>
	/// <typeparam name="TAggregate">Type of your aggregate</typeparam>
	/// <typeparam name="TAggregateId">Type of your aggregate's Id</typeparam>
	/// <typeparam name="TRepositoryInterface">Interface type of your repository</typeparam>
	/// <typeparam name="TRepositoryClass">Actual implementation type of your repository</typeparam>
	/// <typeparam name="TConfiguration">
	///     Custom type of configuration for your repository, useful if you want to append custom
	///     configuration. Your repository constructor should expect this type instead of TableAggregateConfiguration&lt;
	///     TAggregate&gt;.
	/// </typeparam>
	/// <param name="configureOptions">Used to configure the repository via lambda</param>
	public static IServiceCollection AddTableRepository<TAggregate, TAggregateId, TRepositoryInterface,
		TRepositoryClass, TConfiguration>(this IServiceCollection services,
		Action<TConfiguration> configureOptions)
		where TAggregate : notnull
		where TAggregateId : notnull
		where TRepositoryInterface : class
		where TRepositoryClass : TableRepository<TAggregate, TAggregateId>, TRepositoryInterface
		where TConfiguration : TableAggregateConfiguration<TAggregate>, new()
	{
		var ctorInfo = typeof(TRepositoryClass).GetConstructor([typeof(IOptions<TConfiguration>), typeof(IOptions<DefaultConfiguration>)]);
		if (ctorInfo is null || ctorInfo.GetParameters()[0].ParameterType != typeof(IOptions<TConfiguration>))
		{
			throw new ArgumentException($"The constructor for {typeof(TRepositoryClass).Name} does not take IOptions<{typeof(TConfiguration).Name}> as argument!", nameof(configureOptions));
		}

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
}