
namespace Dapper.Repository.IntegrationTests.Aggregates
{
	public record Category
	{
		public int CategoryID { get; init; }

		public string CategoryName { get; init; } = default!;

		public string? Description { get; init; }

		public byte[]? Picture { get; init; }
	}
}
