namespace WeatherService.Api.Models.WeatherStation;
public record WeatherStationViewModel(int Id, string Name, AddressModel Address)
{
	public static WeatherStationViewModel FromEntity(Model.Entities.WeatherStation weatherStation)
	{
		return new WeatherStationViewModel(weatherStation.Id, weatherStation.Name, AddressModel.FromEntity(weatherStation.Address));
	}
}
