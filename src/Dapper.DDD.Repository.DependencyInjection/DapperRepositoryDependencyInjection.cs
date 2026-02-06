namespace Dapper.DDD.Repository.DependencyInjection;

public static partial class DapperRepositoryDependencyInjection
{
	/// <summary>
	///     Configure defaults to use for all aggregate types.
	/// </summary>
	public static IServiceCollection ConfigureDapperRepositoryDefaults(this IServiceCollection services,
		Action<DefaultConfiguration> configureOptions)
	{
		return services.Configure(configureOptions);
	}
}