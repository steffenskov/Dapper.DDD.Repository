using System.Linq;
using Dapper.DDD.Repository.Reflection;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Reflection;

public class ExpressionParserTests
{
	[Fact]
	public void GetExtendedPropertiesFromExpression_SingleProperty_ReturnsProperty()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();

		// Act
		var propertyInfos = parser.GetExtendedPropertiesFromExpression(user => user.Id).ToList();

		// Assert
		Assert.Single(propertyInfos);
		Assert.Equal(nameof(UserAggregate.Id), propertyInfos.Single().Name);
	}

	[Fact]
	public void GetExtendedPropertiesFromExpression_AnonymousObject_ReturnsProperty()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();

		// Act
		var propertyInfos = parser.GetExtendedPropertiesFromExpression(user => new { user.Id }).ToList();

		// Assert
		Assert.Single(propertyInfos);
		Assert.Equal(nameof(UserAggregate.Id), propertyInfos.Single().Name);
	}

	[Fact]
	public void GetExtendedPropertiesFromExpression_AnonymousObjectWithMultipleProperties_ReturnsProperties()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();

		// Act
		var propertyInfos = parser.GetExtendedPropertiesFromExpression(user => new { user.Id, user.InvoiceAddress }).ToList();

		// Assert
		Assert.Equal(2, propertyInfos.Count);
		var propertyNames = propertyInfos.Select(property => property.Name).ToList();
		Assert.Contains(nameof(UserAggregate.Id), propertyNames);
		Assert.Contains(nameof(UserAggregate.InvoiceAddress), propertyNames);
	}

	[Fact]
	public void GetExtendedPropertiesFromExpression_NonAggregateProperty_Throws()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();
		var id = 42;

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() => parser.GetExtendedPropertiesFromExpression(user => id).ToList());
	}

	[Fact]
	public void GetExtendedPropertiesFromExpression_ConstProperty_Throws()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();
		const int id = 42;

		// Act && Assert
		Assert.Throws<NotSupportedException>(() => parser.GetExtendedPropertiesFromExpression(user => id).ToList());
	}

	[Fact]
	public void GetExtendedPropertiesFromExpression_AnonymousObjectWithNonAggregateProperties_Throws()
	{
		// Arrange
		var parser = new ExpressionParser<UserAggregate>();

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() => parser.GetExtendedPropertiesFromExpression(user => new { id = user.Id }).ToList());
	}
}
