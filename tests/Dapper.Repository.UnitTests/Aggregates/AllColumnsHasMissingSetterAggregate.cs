using System;

namespace Dapper.Repository.UnitTests.Aggregates;

public record AllPropertiesHasMissingSetterAggregate
{
	public int Id { get; init; }

	public DateTime DateCreated { get; }
}