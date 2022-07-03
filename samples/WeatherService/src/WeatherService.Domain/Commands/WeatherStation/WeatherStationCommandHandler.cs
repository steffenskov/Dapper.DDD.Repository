using WeatherService.Domain.Repositories;

namespace WeatherService.Domain.Commands.WeatherStation;

internal class WeatherStationCommandHandler : IRequestHandler<WeatherStationCreateCommand, Aggregates.WeatherStation>
{
	private readonly IWeatherStationRepository _repository;
	private readonly IMediator _mediator;

	public WeatherStationCommandHandler(IWeatherStationRepository repository, IMediator mediator)
	{
		_repository = repository;
		_mediator = mediator;
	}

	public async Task<Aggregates.WeatherStation> Handle(WeatherStationCreateCommand request, CancellationToken cancellationToken)
	{
		var aggregate = new Aggregates.WeatherStation();

		await aggregate.OnCreateCommandAsync(request, _mediator, cancellationToken);
		return await _repository.InsertAsync(aggregate, cancellationToken);
	}
}
