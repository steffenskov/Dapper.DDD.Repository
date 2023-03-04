using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Configuration;

public class BaseAggregateConfigurationTests
{
	private class DummyAggregateConfiguration : BaseAggregateConfiguration<UserAggregate>
	{
		protected override string EntityName { get; } = "Users";
	}
	
	[Fact]
	public void HasColumnName_CalledTwiceOnSameProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasColumnName(e => e.Id, "user_id");

		// Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasColumnName(e => e.Id, "user_id"));
		Assert.Equal(@"HasColumnName has already been called once for the property ""Id"".", ex.Message);
	}

	[Fact]
	public void HasColumnName_ReturnsAnonymousTypeContainingMultipleProperties_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasColumnName(e => new { e.Id, e.DeliveryAddress }, "user_id"));
		Assert.Equal("The expression given to HasColumnName must return exactly one property.", ex.Message);
	}

	[Fact]
	public void HasColumnName_ReturnsAnonymousTypeContainingNonProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasColumnName(e => new { NonProperty = e.Id }, "user_id"));
		Assert.Equal("UserAggregate doesn't contain a property named NonProperty.", ex.Message);
	}

	[Fact]
	public void HasColumnName_ReturnsAnonymousTypeContainingSingleProperty_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasColumnName(e => new { e.Id }, "user_id");

		// Assert
		Assert.True(true); // Just checking for exceptions here
	}
	
	[Fact]
	public void HasDefault_ReturnsAnonymousTypeContainingMultipleProperties_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasDefault(e => new { e.Id, e.DeliveryAddress });
		
		// Assert
		Assert.True(true); // Just checking for exceptions here
	}

	[Fact]
	public void HasDefault_ReturnsAnonymousTypeContainingNonProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasDefault(e => new { NonProperty = e.Id }));
		Assert.Equal("UserAggregate doesn't contain a property named NonProperty.", ex.Message);
	}

	[Fact]
	public void HasDefault_ReturnsAnonymousTypeContainingSingleProperty_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasDefault(e => new { e.Id});

		// Assert
		Assert.True(true); // Just checking for exceptions here
	}
	
	[Fact]
	public void HasDefault_CalledTwiceOnSameProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasDefault(e => e.Id);

		// Assert
		var ex = Assert.Throws<ArgumentException>(() => config.HasDefault(e => e.Id ));
		Assert.Equal("One or more items with the same key has already been added.", ex.Message);
	}
	
	[Fact]
	public void HasIdentity_ReturnsAnonymousTypeContainingMultipleProperties_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasIdentity(e => new { e.Id, e.DeliveryAddress });
		
		// Assert
		Assert.True(true); // Just checking for exceptions here
	}

	[Fact]
	public void HasIdentity_ReturnsAnonymousTypeContainingNonProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasIdentity(e => new { NonProperty = e.Id }));
		Assert.Equal("UserAggregate doesn't contain a property named NonProperty.", ex.Message);
	}

	[Fact]
	public void HasIdentity_ReturnsAnonymousTypeContainingSingleProperty_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasIdentity(e => new { e.Id});

		// Assert
		Assert.True(true); // Just checking for exceptions here
	}
	
	[Fact]
	public void HasIdentity_CalledTwiceOnSameProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasIdentity(e => e.Id);

		// Assert
		var ex = Assert.Throws<ArgumentException>(() => config.HasIdentity(e => e.Id ));
		Assert.Equal("One or more items with the same key has already been added.", ex.Message);
	}

	[Fact]
	public void HasKey_CalledTwice_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasKey(e => e.Id);
		
		// Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasKey(e => e.Id));
		Assert.Equal("HasKey has already been called once.", ex.Message);
	}

	[Fact]
	public void HasKey_ReturnsAnonymousTypeContainingMultipleProperties_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasKey(e => new { e.Id, e.DeliveryAddress });
		
		// Assert
		Assert.True(true); // Just checking for exceptions here
	}

	[Fact]
	public void HasKey_ReturnsAnonymousTypeContainingNonProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.HasKey(e => new { NonProperty = e.Id }));
		Assert.Equal("UserAggregate doesn't contain a property named NonProperty.", ex.Message);
	}

	[Fact]
	public void HasKey_ReturnsAnonymousTypeContainingSingleProperty_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.HasKey(e => new { e.Id});

		// Assert
		Assert.True(true); // Just checking for exceptions here
	}
	
	[Fact]
	public void Ignore_ReturnsAnonymousTypeContainingMultipleProperties_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.Ignore(e => new { e.Id, e.DeliveryAddress });
		
		// Assert
		Assert.True(true); // Just checking for exceptions here
	}

	[Fact]
	public void Ignore_ReturnsAnonymousTypeContainingNonProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => config.Ignore(e => new { NonProperty = e.Id }));
		Assert.Equal("UserAggregate doesn't contain a property named NonProperty.", ex.Message);
	}

	[Fact]
	public void Ignore_ReturnsAnonymousTypeContainingSingleProperty_Valid()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.Ignore(e => new { e.Id});

		// Assert
		Assert.True(true); // Just checking for exceptions here
	}
	
	[Fact]
	public void Ignore_CalledTwiceOnSameProperty_Throws()
	{
		// Arrange
		var config = new DummyAggregateConfiguration();
		
		// Act
		config.Ignore(e => e.Id);

		// Assert
		var ex = Assert.Throws<ArgumentException>(() => config.Ignore(e => e.Id ));
		Assert.Equal("One or more items with the same key has already been added.", ex.Message);
	}
}