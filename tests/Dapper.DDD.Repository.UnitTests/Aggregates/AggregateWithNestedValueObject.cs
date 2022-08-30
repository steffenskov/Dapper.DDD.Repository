using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Aggregates;

public record AggregateWithNestedValueObject(Guid Id, FirstLevelValueObject FirstLevel);