using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper.DDD.Repository.Reflection;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Reflection;

public class ExtendedPropertyInfoCollectionTests
{
	[Fact]
	public void Remove_PropertyWithNameExists_IsRemovedFromBothListAndDictionary()
	{
		// Arrange
		var collection = CreateCollection();

		var propCount = collection.Count;

		var propToRemove = typeof(UserAggregate).GetProperties(BindingFlags.Instance | BindingFlags.Public).First();

		// Act
		collection.Remove(new ExtendedPropertyInfo(propToRemove));

		// Assert
		Assert.Equal(propCount - 1, collection.Count);
		foreach (var prop in collection) // Uses the list
		{
			Assert.True(collection.Contains(prop)); // Uses the dictionary
		}
	}

	[Fact]
	public void Add_PropertyWithNameExists_Throws()
	{
		// Arrange
		var collection = CreateCollection();

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => collection.Add(collection.First()));
		Assert.Contains("An item with the same key has already been added.", ex.Message);
	}

	[Fact]
	public void AddRange_PropertyWithNameExists_Throws()
	{
		// Arrange
		var collection = CreateCollection();

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => collection.AddRange(collection));
		Assert.Contains("An item with the same key has already been added.", ex.Message);
	}

	[Fact]
	public void TryGetValue_PropertyWithNameExists_IsFetched()
	{
		// Arrange
		var collection = CreateCollection();

		// Act
		var isFetched = collection.TryGetValue(collection.First().Name, out var extendedPropertyInfo);

		// Assert
		Assert.True(isFetched);
		Assert.NotNull(extendedPropertyInfo);
	}

	[Fact]
	public void TryGetValue_PropertyWithNameDoesNotExists_ReturnsFalse()
	{
		// Arrange
		var collection = CreateCollection();

		// Act
		var isFetched = collection.TryGetValue("PropertyNameThatDoesNotExist", out var extendedPropertyInfo);

		// Assert
		Assert.False(isFetched);
		Assert.Null(extendedPropertyInfo);
	}

	[Fact]
	public void IndexerWithInt_OutOfRange_Throws()
	{
		// Arrange
		var collection = CreateCollection();

		// Act && Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => collection[int.MaxValue]);
	}

	[Fact]
	public void IndexerWithInt_InRange_ReturnsProperty()
	{
		// Arrange
		var collection = CreateCollection();

		// Act
		var property = collection[collection.Count - 1];

		// Assert
		Assert.NotNull(property);
	}

	[Fact]
	public void IndexerWithString_OutOfRange_Throws()
	{
		// Arrange
		var collection = CreateCollection();

		// Act && Assert
		Assert.Throws<KeyNotFoundException>(() => collection["PropertyNameThatDoesNotExist"]);
	}

	[Fact]
	public void IndexerWithString_InRange_ReturnsProperty()
	{
		// Arrange
		var collection = CreateCollection();

		// Act
		var property = collection[collection.First().Name];

		// Assert
		Assert.NotNull(property);
	}

	private static ExtendedPropertyInfoCollection CreateCollection()
	{
		var collection = new ExtendedPropertyInfoCollection();
		var rawProps = typeof(UserAggregate).GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (var prop in rawProps)
		{
			collection.Add(new ExtendedPropertyInfo(prop));
		}

		return collection;
	}
}