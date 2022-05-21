using System;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.UnitTests.Entities
{
	internal record CompositePrimaryKeyEntity : DbEntity
	{
		[PrimaryKeyColumn]
		public string Username { get; init; } = default!;

		[PrimaryKeyColumn]
		public string Password { get; init; } = default!;

		[Column]
		public DateTime DateCreated { get; init; }
	}
}
