using System.Linq;
using System.Threading.Tasks;
using Dapper.DDD.Repository.Interfaces;
using NetTopologySuite.Geometries;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class SerializedColumnTests : IClassFixture<Startup>
{
	private readonly ITableRepository<City, Guid> _repository;

	public SerializedColumnTests(Startup startup)
	{
		_repository = startup.Provider.GetService<ITableRepository<City, Guid>>()!;
		var foo = 42;
	}

	[Fact]
	public async Task Delete_Valid_GeometryIsIncludedInReturnValue()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Geometry.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
			{
				new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)
			})
		};
		city.GeoLocation.SRID = 25832;
		city.Area.SRID = 25832;

		await _repository.InsertAsync(city);

		// Act
		var result = await _repository.DeleteAsync(city.Id);

		// Assert
		Assert.Equal(city, result);
	}

	[Fact]
	public async Task Insert_Valid_GeometryIsIncludedInReturnValue()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Geometry.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
			{
				new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)
			})
		};
		city.GeoLocation.SRID = 25832;
		city.Area.SRID = 25832;

		// Act
		var result = await _repository.InsertAsync(city);

		// Assert
		try
		{
			Assert.Equal(city.GeoLocation, result.GeoLocation);
			Assert.Equal(25832, result.GeoLocation.SRID);
			Assert.Equal(25832, result.Area.SRID);
		}
		finally
		{
			await _repository.DeleteAsync(city.Id);
		}
	}

	[Fact]
	public async Task GetAll_Valid_GeometryIsIncludedInReturnValue()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Geometry.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
			{
				new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)
			})
		};
		city.GeoLocation.SRID = 25832;
		city.Area.SRID = 25832;
		await _repository.InsertAsync(city);

		// Act
		var result = (await _repository.GetAllAsync()).ToList();

		// Assert
		try
		{
			Assert.Contains(result, item => item.Id == city.Id);
			var fetchedCity = result.Single(item => item.Id == city.Id);
			Assert.Equal(city, fetchedCity);
		}
		finally
		{
			await _repository.DeleteAsync(city.Id);
		}
	}

	[Fact]
	public async Task Get_Valid_GeometryIsIncludedInReturnValue()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Geometry.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
			{
				new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)
			})
		};
		city.GeoLocation.SRID = 25832;
		city.Area.SRID = 25832;
		await _repository.InsertAsync(city);

		// Act
		var fetchedCity = await _repository.GetAsync(city.Id);

		// Assert
		try
		{
			Assert.Equal(city, fetchedCity);
		}
		finally
		{
			await _repository.DeleteAsync(city.Id);
		}
	}

	[Fact]
	public async Task Update_Valid_GeometryIsIncludedInReturnValue()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Geometry.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
			{
				new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)
			})
		};
		city.GeoLocation.SRID = 25832;
		city.Area.SRID = 25832;
		await _repository.InsertAsync(city);

		// Act
		var newArea = Geometry.DefaultFactory.CreatePolygon(new Coordinate[]
		{
			new(10, 10), new(10, 20), new(20, 20), new(20, 10), new(10, 10)
		});
		newArea.SRID = 1234;
		var updatedCity = await _repository.UpdateAsync(city with { Area = newArea });

		// Assert
		try
		{
			Assert.NotNull(updatedCity);
			Assert.Equal(newArea, updatedCity!.Area);
		}
		finally
		{
			await _repository.DeleteAsync(city.Id);
		}
	}
}