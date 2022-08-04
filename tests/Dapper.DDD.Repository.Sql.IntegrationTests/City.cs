using NetTopologySuite.Geometries;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public record City
{
	public Guid Id { get; set; }
	public Point GeoLocation { get; set; } = default!;
	public Polygon Area { get; set; } = default!;
}
