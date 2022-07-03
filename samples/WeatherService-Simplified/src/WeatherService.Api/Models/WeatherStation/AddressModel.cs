using WeatherService.Model.ValueObjects;

namespace WeatherService.Api.Models.WeatherStation;
public record AddressModel(string Street, string City)
{
	public static AddressModel FromEntity(Address address)
	{
		return new AddressModel(address.Street, address.City);
	}
}
