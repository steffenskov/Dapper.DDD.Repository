using Dapper.DDD.Repository.Interfaces;
using NetTopologySuite.Geometries;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class GeometryTests : IClassFixture<Startup>
{
	private readonly ITableRepository<City, Guid> _repository;

	public GeometryTests(Startup startup)
	{
		_repository = startup.Provider.GetService<ITableRepository<City, Guid>>()!;
	}

	[Fact]
	public async Task Insert_Valid_PointIsKept()
	{
		// Arrange
		var city = new City
		{
			Id = Guid.NewGuid(),
			GeoLocation = Point.DefaultFactory.CreatePoint(new Coordinate(42, 1337)),
			Area = Polygon.DefaultFactory.CreatePolygon(new Coordinate[] { new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1) })
		};
		city.GeoLocation.SRID = 28532;
		city.Area.SRID = 28532;

		// Act
		var result = await _repository.InsertAsync(city);

		// Assert
		Assert.Equal(city.GeoLocation, result.GeoLocation);
		Assert.Equal(28532, result.GeoLocation.SRID);
		Assert.Equal(28532, result.Area.SRID);
	}
}
