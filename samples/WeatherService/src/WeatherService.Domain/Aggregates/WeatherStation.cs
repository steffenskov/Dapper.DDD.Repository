using WeatherService.Domain.Commands.WeatherStation;

namespace WeatherService.Domain.Aggregates;

public class WeatherStation
{
	public int Id { get; private set; }
	public string Name { get; private set; } = default!;
	public Address Address { get; private set; } = default!;

	public async Task Create(WeatherStationCreateCommand command, IMediator mediator, CancellationToken cancellationToken)
	{
		await ValidateNameAsync(command.Name, mediator, cancellationToken);

		Name = command.Name;
		Address = command.Address;
	}

	private static Task ValidateNameAsync(string name, IMediator mediator, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
