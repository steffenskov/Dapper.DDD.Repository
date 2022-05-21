using System;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.IntegrationTests.Entities
{
	public record CompositeUserPrimaryKeyEntity : DbEntity
	{
		[PrimaryKeyColumn(isIdentity: false)]
		public string Username { get; init; } = default!;

		[PrimaryKeyColumn(isIdentity: false)]
		public string Password { get; init; } = default!;
	}

	public record CompositeUserEntity : CompositeUserPrimaryKeyEntity
	{
		[Column(hasDefaultConstraint: true)]
		public DateTime DateCreated { get; } // No init; as I want this value to never be set by the user

		[Column]
		public int? Age { get; init; }

	}
}
