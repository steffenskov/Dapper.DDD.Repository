using Dapper.Repository.Reflection;

namespace Dapper.Repository.UnitTests.Reflection;

public class ObjectFlattenerTests
{
	[Fact]
	public void Flatten_ObjectWithSimpleProperties_ReturnsSameObject()
	{
		// Arrange
		var obj = new { Id = 1, Name = "Test" };

		// Act
		var result = ObjectFlattener.Flatten(obj);

		// Assert
		Assert.Same(obj, result);
	}

	[Fact]
	public void Flatten_SimpleType_ReturnsSameObject()
	{
		// Arrange
		var obj = "Hello world";

		// Act
		var result = ObjectFlattener.Flatten(obj);

		// Assert
		Assert.Same(obj, result);
	}

	[Fact]
	public void Flatten_ComplexType_ReturnsFlatObject()
	{
		// Arrange
		var obj = new
		{
			Id = Guid.NewGuid(),
			Name = "Test",
			Address = new
			{
				Street = "Test street",
				City = "Test city"
			}
		};

		// Act
		var result = ObjectFlattener.Flatten(obj);
		var resultType = result.GetType();

		object getResultValue(string name)
		{
			return resultType!.GetProperty(name)!.GetValue(result)!;
		}

		// Assert
		Assert.NotSame(obj, result);
		Assert.Equal(obj.Id, getResultValue("Id"));
		Assert.Equal(obj.Name, getResultValue("Name"));
		Assert.Equal(obj.Address.Street, getResultValue("Address_Street"));
		Assert.Equal(obj.Address.City, getResultValue("Address_City"));
	}
}
