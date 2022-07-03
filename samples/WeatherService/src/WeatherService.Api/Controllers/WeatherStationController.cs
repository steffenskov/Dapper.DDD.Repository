using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.Models.WeatherStation;
using WeatherService.Domain.Commands.WeatherStation;
using WeatherService.Domain.Queries.WeatherStation;
using WeatherService.Domain.ValueObjects;

namespace WeatherService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherStationController : ControllerBase
{
	private readonly IMediator _mediator;

	public WeatherStationController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<ActionResult<WeatherStationViewModel>> CreateWeatherStationAsync(WeatherStationCreateModel model, CancellationToken cancellationToken)
	{
		var command = new WeatherStationCreateCommand(model.Name, new Address
		{
			City = model.Address.City,
			Street = model.Address.Street
		});

		var result = await _mediator.Send(command, cancellationToken);

		return Created($"/WeatherStation/{result.Id}", WeatherStationViewModel.FromDomainModel(result));
	}

	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<WeatherStationViewModel?>> GetAsync(int id, CancellationToken cancellationToken)
	{
		var query = new WeatherStationGetSingleQuery(id);

		var result = await _mediator.Send(query, cancellationToken);

		return result is not null
			? WeatherStationViewModel.FromDomainModel(result)
			: NotFound();
	}

	[HttpGet]
	public async Task<ActionResult<IList<WeatherStationViewModel>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var query = new WeatherStationGetAllQuery();

		var result = await _mediator.Send(query, cancellationToken);
		return result.Select(WeatherStationViewModel.FromDomainModel).ToList();
	}
}
