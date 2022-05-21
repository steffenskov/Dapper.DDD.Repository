using System;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.UnitTests.Entities
{
	public record ColumnHasMissingSetterEntity : DbEntity
	{
		[PrimaryKeyColumn]
		public int Id { get; init; }

		[Column]
		public int Age { get; init; }

		[Column(hasDefaultConstraint: true)]
		public DateTime DateCreated { get; }
	}
}