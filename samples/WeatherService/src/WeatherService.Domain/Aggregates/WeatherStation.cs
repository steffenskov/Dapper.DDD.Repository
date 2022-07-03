using WeatherService.Domain.Commands.WeatherStation;
using WeatherService.Domain.Queries.WeatherStation;

namespace WeatherService.Domain.Aggregates;

public class WeatherStation
{
	public int Id { get; private init; } // init because it's an identity field in the database, as such the code should never set it explicitly
	public string Name { get; private set; } = default!;
	public Address Address { get; private set; } = default!;

	public async Task OnCreateCommandAsync(WeatherStationCreateCommand command, IMediator mediator, CancellationToken cancellationToken)
	{
		await ValidateNameAsync(command.Name, mediator, cancellationToken); // In Domain Driven design we validate everything prior to setting properties to ensure an aggregate never enters a partially invalid state.

		Name = command.Name;
		Address = command.Address;
	}

	/// <summary>
	/// Validate that name against the business rules. In this case those rules are length and uniqueness.
	/// </summary>
	private static async Task ValidateNameAsync(string name, IMediator mediator, CancellationToken cancellationToken)
	{
		if (name.Length > 50)
		{
			throw new ArgumentOutOfRangeException(nameof(name), $"{nameof(Name)} cannot exceed 50 characters");
		}

		var query = new WeatherStationNameInUseQuery(name);
		var nameInUse = await mediator.Send(query, cancellationToken);
		if (nameInUse)
		{
			throw new ArgumentException($@"The name ""{name}"" is already used.", nameof(name));
		}
	}
}
