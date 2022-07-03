using Microsoft.Extensions.DependencyInjection;

namespace WeatherService.Infrastructure;

public static class Setup
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		return services;
	}
}
