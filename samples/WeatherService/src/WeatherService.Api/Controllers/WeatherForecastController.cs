using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.Models.WeatherForecast;
using WeatherService.Domain.Commands.WeatherForecast;
using WeatherService.Domain.Queries.WeatherForecast;

namespace WeatherService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private readonly IMediator _mediator;

	public WeatherForecastController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<ActionResult<IList<WeatherForecastViewModel>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var query = new WeatherForecastGetLatestQuery();

		var result = await _mediator.Send(query, cancellationToken);

		return result is not null
				? result.Select(WeatherForecastViewModel.FromDomainModel).ToList()
				: NotFound();
	}

	[HttpPost]
	public async Task<ActionResult<WeatherForecastViewModel>> CreateAsync(WeatherForecastCreateModel model, CancellationToken cancellationToken)
	{
		var command = new WeatherForecastCreateCommand(model.WeatherStationId, model.TemperatureC, model.Summary);

		var result = await _mediator.Send(command, cancellationToken);

		return CreatedAtAction(nameof(GetAsync), WeatherForecastViewModel.FromDomainModel(result));
	}

	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<WeatherForecastViewModel?>> GetAsync(long id, CancellationToken cancellationToken)
	{
		var query = new WeatherForecastGetSingleQuery(id);

		var result = await _mediator.Send(query, cancellationToken);

		return result is not null
			? WeatherForecastViewModel.FromDomainModel(result)
			: NotFound();
	}
}
