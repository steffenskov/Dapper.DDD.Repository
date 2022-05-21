using Dapper.Repository.Attributes;

namespace Dapper.Repository.IntegrationTests.Entities
{
	public record CategoryPrimaryKeyEntity : DbEntity
	{
		[PrimaryKeyColumn(true, "CategoryID")]
		public int CategoryId { get; init; }
	}

	public record CategoryEntity : CategoryPrimaryKeyEntity
	{
		[Column("CategoryName")]
		public string Name { get; init; } = default!;

		[Column]
		public string? Description { get; init; }

		[Column]
		public byte[]? Picture { get; init; }
	}
}
