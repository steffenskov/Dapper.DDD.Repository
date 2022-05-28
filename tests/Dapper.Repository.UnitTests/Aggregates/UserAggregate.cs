using System;
using Dapper.Repository.UnitTests.ValueObjects;

namespace Dapper.Repository.UnitTests.Aggregates;
public record UserAggregate
{
	public Guid Id { get; init; }
	public Address Address { get; init; } = default!;
}