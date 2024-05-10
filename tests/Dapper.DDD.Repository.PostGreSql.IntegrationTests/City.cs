namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public record City
{
	public Guid Id { get; set; }
	public Point GeoLocation { get; set; } = default!;
	public Polygon Area { get; set; } = default!;
}