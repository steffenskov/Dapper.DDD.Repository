using System;
using Dapper.Repository.Configuration;
using Dapper.Repository.Sql;
using Dapper.Repository.UnitTests.Aggregates;
using Xunit;

namespace Dapper.Repository.UnitTests.Sql;

public class SqlQueryGeneratorFactoryTests
{
	[Fact]
	public void Create_NotSqlAggregateConfiguration_Throws()
	{
		// Arrange
		var factory = new SqlQueryGeneratorFactory();
		var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>(default);

		// Act && Assert
		Assert.Throws<ArgumentException>(() => factory.Create(configuration));
	}
}