namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;
public record ProductListView
{
	public int ProductID { get; }

	public string ProductName { get; } = default!;
}
