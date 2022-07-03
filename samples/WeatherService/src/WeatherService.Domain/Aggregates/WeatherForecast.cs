using WeatherService.Domain.Commands.WeatherForecast;
using WeatherService.Domain.Enums;
using WeatherService.Domain.Queries.WeatherStation;

namespace WeatherService.Domain.Aggregates;

public class WeatherForecast
{
	public long Id { get; private init; } // init because it's an identity field in the database, as such the code should never set it explicitly
	public int WeatherStationId { get; private set; }
	public int TemperatureC { get; private set; }
	public WeatherForecastSummary Summary { get; private set; }
	public DateTime Timestamp { get; private init; } // init because it's being set by a default constraint in the database, as such the code should never set it explicitly

	internal async Task OnCreateCommandAsync(WeatherForecastCreateCommand request, IMediator mediator, CancellationToken cancellationToken)
	{
		ValidateSummary(request.Summary);
		await ValidateWeatherStationId(request.WeatherStationId, mediator, cancellationToken);

		WeatherStationId = request.WeatherStationId;
		TemperatureC = request.TemperatureC;
		Summary = request.Summary;
	}

	private static void ValidateSummary(WeatherForecastSummary summary)
	{
		if (!Enum.IsDefined(summary))
		{
			throw new ArgumentOutOfRangeException(nameof(summary), $"The summary value is invalid");
		}
	}

	private static async Task ValidateWeatherStationId(int weatherStationId, IMediator mediator, CancellationToken cancellationToken)
	{
		var weatherStationQuery = new WeatherStationGetSingleQuery(weatherStationId);

		var weatherStation = await mediator.Send(weatherStationQuery, cancellationToken);

		if (weatherStation is null)
		{
			throw new ArgumentOutOfRangeException(nameof(weatherStationId), $"No weatherstation exists with the id {weatherStationId}");
		}
	}
}
