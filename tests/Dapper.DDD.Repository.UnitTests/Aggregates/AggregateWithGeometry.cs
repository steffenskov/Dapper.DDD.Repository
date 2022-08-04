using NetTopologySuite.Geometries;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public class AggregateWithGeometry
{
	public Guid Id { get; set; }
	public Polygon Area { get; set; } = default!;
}
