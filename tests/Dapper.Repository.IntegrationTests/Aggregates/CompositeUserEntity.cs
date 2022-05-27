using System;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.IntegrationTests.Aggregates
{
	public record CompositeUserPrimaryKeyAggregate
	{
		[PrimaryKeyColumn(isIdaggregate: false)]
		public string Username { get; init; } = default!;

		[PrimaryKeyColumn(isIdaggregate: false)]
		public string Password { get; init; } = default!;
	}

	public record CompositeUserAggregate : CompositeUserPrimaryKeyAggregate
	{
		[Column(hasDefaultConstraint: true)]
		public DateTime DateCreated { get; } // No init; as I want this value to never be set by the user

		[Column]
		public int? Age { get; init; }

	}
}
