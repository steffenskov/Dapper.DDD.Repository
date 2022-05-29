using System;

namespace Dapper.Repository.UnitTests.Aggregates
{
	public record PropertyHasMissingSetterAggregate
	{
		public int Id { get; set; }

		public int Age { get; set; }

		public DateTime DateCreated { get; }
	}
}