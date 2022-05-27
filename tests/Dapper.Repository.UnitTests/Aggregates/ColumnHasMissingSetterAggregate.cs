using System;

namespace Dapper.Repository.UnitTests.Aggregates
{
	public record PropertyHasMissingSetterAggregate
	{
		public int Id { get; init; }

		public int Age { get; init; }

		public DateTime DateCreated { get; }
	}
}