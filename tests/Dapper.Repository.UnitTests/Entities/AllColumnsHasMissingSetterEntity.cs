using System;

namespace Dapper.Repository.UnitTests.Aggregates
{
	public record AllColumnsHasMissingSetterAggregate
	{
		public int Id { get; init; }

		public DateTime DateCreated { get; }
	}
}