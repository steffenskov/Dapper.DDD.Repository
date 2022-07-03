namespace WeatherService.Model.ValueObjects;

/// <summary>
/// Address is a ValueObject that encapsulates addresses.
/// As such it handles validation of its own properties directly.
/// </summary>
public record Address
{
	private string _street = default!;

	public string Street
	{
		get => _street;
		init
		{
			if (value.Length > 100)
			{
				throw new ArgumentOutOfRangeException(nameof(Street), $"{nameof(Street)} cannot exceed 100 characters");
			}

			_street = value;
		}
	}

	private string _city = default!;
	public string City
	{
		get => _city;
		init
		{
			if (value.Length > 50)
			{
				throw new ArgumentOutOfRangeException(nameof(City), $"{nameof(City)} cannot exceed 50 characters");
			}

			_city = value;
		}
	}
}
