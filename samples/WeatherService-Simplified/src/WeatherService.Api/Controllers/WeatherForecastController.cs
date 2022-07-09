using Dapper.DDD.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.Models.WeatherForecast;
using WeatherService.Model.Entities;
using WeatherService.Model.Repositories;

namespace WeatherService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private readonly ITableRepository<WeatherForecast, long> _tableRepository;
	private readonly IWeatherForecastViewRepository _viewRepository;
	private readonly IWeatherStationRepository _weatherStationRepository;

	public WeatherForecastController(ITableRepository<WeatherForecast, long> tableRepository, IWeatherForecastViewRepository viewRepository, IWeatherStationRepository weatherStationRepository)
	{
		_tableRepository = tableRepository;
		_viewRepository = viewRepository;
		_weatherStationRepository = weatherStationRepository;
	}

	[HttpGet]
	public async Task<ActionResult<IList<WeatherForecastViewModel>>> GetLatestAsync(CancellationToken cancellationToken)
	{
		var result = await _viewRepository.GetLatestAsync(cancellationToken);

		return result is not null
				? result.Select(WeatherForecastViewModel.FromEntity).ToList()
				: NotFound();
	}

	[HttpPost]
	public async Task<ActionResult<WeatherForecastViewModel>> CreateAsync(WeatherForecastCreateModel model, CancellationToken cancellationToken)
	{
		if (!Enum.IsDefined(model.Summary))
		{
			throw new ArgumentOutOfRangeException(nameof(model), $"The summary value is invalid");
		}

		var weatherStation = await _weatherStationRepository.GetAsync(model.WeatherStationId, cancellationToken);

		if (weatherStation is null)
		{
			throw new ArgumentOutOfRangeException(nameof(model), $"No weatherstation exists with the id {model.WeatherStationId}");
		}

		var created = await _tableRepository.InsertAsync(new WeatherForecast
		{
			Summary = model.Summary,
			TemperatureC = model.TemperatureC,
			WeatherStationId = model.WeatherStationId
		}, cancellationToken);


		var result = (await _viewRepository.GetAsync(created.Id, cancellationToken))!;

		return Created($"/WeatherForecast/{result.Id}", WeatherForecastViewModel.FromEntity(result));
	}

	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<WeatherForecastViewModel?>> GetAsync(long id, CancellationToken cancellationToken)
	{
		var result = await _viewRepository.GetAsync(id, cancellationToken);

		return result is not null
			? WeatherForecastViewModel.FromEntity(result)
			: NotFound();
	}
}
