using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.Models.WeatherStation;
using WeatherService.Model.Repositories;

namespace WeatherService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherStationController : ControllerBase
{
	private readonly IWeatherStationRepository _repository;

	public WeatherStationController(IWeatherStationRepository repository)
	{
		_repository = repository;
	}

	[HttpPost]
	public async Task<ActionResult<WeatherStationViewModel>> CreateWeatherStationAsync(WeatherStationCreateModel model, CancellationToken cancellationToken)
	{
		if (model.Name.Length > 50)
		{
			throw new ArgumentOutOfRangeException(nameof(model.Name), $"{nameof(model.Name)} cannot exceed 50 characters");
		}

		if (await _repository.NameInUseAsync(model.Name, cancellationToken))
		{
			throw new ArgumentException($@"The name ""{model.Name}"" is already used.", nameof(model));
		}

		var result = await _repository.InsertAsync(new Model.Entities.WeatherStation
		{
			Name = model.Name,
			Address = new Model.ValueObjects.Address
			{
				City = model.Address.City,
				Street = model.Address.Street
			}
		}, cancellationToken);

		return Created($"/WeatherStation/{result.Id}", WeatherStationViewModel.FromEntity(result));
	}

	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<WeatherStationViewModel?>> GetAsync(int id, CancellationToken cancellationToken)
	{
		var result = await _repository.GetAsync(id, cancellationToken);

		return result is not null
					? WeatherStationViewModel.FromEntity(result)
					: NotFound();
	}

	[HttpGet]
	public async Task<ActionResult<IList<WeatherStationViewModel>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var result = await _repository.GetAllAsync(cancellationToken);

		return result
					.Select(WeatherStationViewModel.FromEntity)
					.ToList();
	}
}
