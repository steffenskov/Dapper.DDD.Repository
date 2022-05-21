using Dapper.Repository.Attributes;

namespace Dapper.Repository.IntegrationTests.Entities
{
	public record ProductListViewEntity : DbEntity
	{
		[Column]
		public int ProductID { get; }

		[Column]
		public string ProductName { get; } = default!;
	}
}
