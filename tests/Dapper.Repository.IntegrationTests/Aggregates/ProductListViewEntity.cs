using Dapper.Repository.Attributes;

namespace Dapper.Repository.IntegrationTests.Aggregates
{
	public record ProductListViewAggregate
	{
		[Column]
		public int ProductID { get; }

		[Column]
		public string ProductName { get; } = default!;
	}
}
