namespace WeatherService.Api.Models.WeatherStation;
public record WeatherStationViewModel(int Id, string Name, AddressModel Address)
{
	public static WeatherStationViewModel FromDomainModel(Domain.Aggregates.WeatherStation weatherStation)
	{
		return new WeatherStationViewModel(weatherStation.Id, weatherStation.Name, AddressModel.FromDomainModel(weatherStation.Address));
	}
}
