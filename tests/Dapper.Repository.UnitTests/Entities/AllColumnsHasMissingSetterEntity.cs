using System;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.UnitTests.Entities
{
	public record AllColumnsHasMissingSetterEntity : DbEntity
	{
		[PrimaryKeyColumn]
		public int Id { get; init; }

		[Column(hasDefaultConstraint: true)]
		public DateTime DateCreated { get; }
	}
}