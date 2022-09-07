using StrongTypedId;

namespace Dapper.DDD.Repository.IntegrationTests.Aggregates;

public class CategoryId : StrongTypedId<CategoryId, int>
{
	public CategoryId(int primitiveId) : base(primitiveId)
	{
	}
}

public record Category
{
	public CategoryId CategoryID { get; init; } = default!;

	public string CategoryName { get; init; } = default!;

	public string? Description { get; init; }

	public byte[]? Picture { get; init; }
}