using WeatherService.Domain.ValueObjects;

namespace WeatherService.Api.Models.WeatherStation;
public record AddressModel(string Street, string City)
{
	public static AddressModel FromDomainModel(Address address)
	{
		return new AddressModel(address.Street, address.City);
	}
}
