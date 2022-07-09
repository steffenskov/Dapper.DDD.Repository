using Dapper.DDD.Repository.Interfaces;
using WeatherService.Domain.Queries.WeatherForecast;

namespace WeatherService.Domain.Commands.WeatherForecast;

internal class WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCreateCommand, WeatherForecastView>
{
	private readonly ITableRepository<Aggregates.WeatherForecast, long> _tableRepository;
	private readonly IMediator _mediator;

	public WeatherForecastCommandHandler(ITableRepository<Aggregates.WeatherForecast, long> tableRepository, IMediator mediator)
	{
		_tableRepository = tableRepository;
		_mediator = mediator;
	}

	public async Task<WeatherForecastView> Handle(WeatherForecastCreateCommand request, CancellationToken cancellationToken)
	{
		var aggregate = new Aggregates.WeatherForecast();
		await aggregate.OnCreateCommandAsync(request, _mediator, cancellationToken);

		var savedResult = await _tableRepository.InsertAsync(aggregate, cancellationToken);

		var getViewQuery = new WeatherForecastGetSingleQuery(savedResult.Id);

		return (await _mediator.Send(getViewQuery, cancellationToken))!;
	}
}
