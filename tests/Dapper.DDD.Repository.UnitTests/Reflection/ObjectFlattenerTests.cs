using Dapper.DDD.Repository.Reflection;
using Dapper.DDD.Repository.UnitTests.Aggregates;

namespace Dapper.DDD.Repository.UnitTests.Reflection;

public class ObjectFlattenerTests
{
	public class NoDefaultConstructorWithPrivateSetterProperty
	{
		public int Age { get; private set; }
		public NoDefaultConstructorWithPrivateSetterProperty(int age)
		{
			Age = age;
		}
	}

	public class PropertiesWithNoSetter
	{
		public int Age { get; }
		public string Name { get; }
		public double? Factor { get; }
		public PropertiesWithNoSetter(int age, string name, double? factor)
		{
			Age = age;
			Name = name;
			Factor = factor;
		}
	}

	[Fact]
	public void Flatten_ObjectWithSimpleProperties_ReturnsSameObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new { Id = 1, Name = "Test" };

		// Act
		var result = objectFlattener.Flatten(obj);

		// Assert
		Assert.Same(obj, result);
	}

	[Fact]
	public void Flatten_SimpleType_ReturnsSameObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = "Hello world";

		// Act
		var result = objectFlattener.Flatten(obj);

		// Assert
		Assert.Same(obj, result);
	}

	[Fact]
	public void Flatten_ComplexType_ReturnsFlatObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
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
		var result = objectFlattener.Flatten(obj);
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

	[Fact]
	public void AddTypeConverter_Exists_Throws()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid)));

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid))));

		Assert.Equal($"A TypeConverter for the type {typeof(StrongUserId)} has already been added.", ex.Message);
	}

	[Fact]
	public void Flatten_HasCustomType_ReturnsFlatObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new UserWithStrongTypedId
		{
			Id = new StrongUserId(42),
			Username = "Some name"
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid)));

		// Act
		var result = objectFlattener.Flatten(obj);

		var resultType = result.GetType();

		object getResultValue(string name)
		{
			return resultType!.GetProperty(name)!.GetValue(result)!;
		}

		// Assert
		Assert.NotSame(obj, result);
		Assert.Equal(obj.Id.PrimitiveId, getResultValue("Id"));
		Assert.Equal(obj.Username, getResultValue("Username"));
	}

	[Fact]
	public void Flatten_HasCustomTypeInValueObject_ReturnsFlatObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new CustomerWithUser
		{
			Id = 42,
			User = new ValueObjects.UserValueObject { Id = new StrongUserId(1337), Username = "Some user" }
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid)));

		// Act
		var result = objectFlattener.Flatten(obj);

		var resultType = result.GetType();

		object getResultValue(string name)
		{
			return resultType!.GetProperty(name)!.GetValue(result)!;
		}

		// Assert
		Assert.NotSame(obj, result);
		Assert.Equal(obj.Id, getResultValue("Id"));
		Assert.Equal(obj.User.Id, getResultValue("User_Id"));
		Assert.Equal(obj.User.Username, getResultValue("User_Username"));
	}

	[Fact]
	public void Unflatten_HasCustomType_ReturnsComplexObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new UserWithStrongTypedId
		{
			Id = new StrongUserId(42),
			Username = "Some name"
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid)));
		var flat = objectFlattener.Flatten(obj);

		// Act
		var complex = objectFlattener.Unflatten<UserWithStrongTypedId>(flat);

		// Assert
		Assert.NotSame(obj, complex);
		Assert.Equal(obj.Id.PrimitiveId, complex.Id!.PrimitiveId);
		Assert.Equal(obj.Username, complex.Username);
	}

	[Fact]
	public void Unflatten_HasCustomTypeInValueObject_ReturnsComplexObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new CustomerWithUser
		{
			Id = 42,
			User = new ValueObjects.UserValueObject { Id = new StrongUserId(1337), Username = "Some user" }
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId), new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveId, uid => new StrongUserId(uid)));
		var flat = objectFlattener.Flatten(obj);

		// Act
		var complex = objectFlattener.Unflatten<CustomerWithUser>(flat);

		// Assert
		Assert.NotSame(obj, complex);
		Assert.Equal(obj.Id, complex.Id);
		Assert.Equal(obj.User.Id, complex.User.Id);
		Assert.Equal(obj.User.Username, complex.User.Username);
	}

	[Fact]
	public void Unflatten_HasNoDefaultConstructorAndPropertyWithPrivateSetter_ReturnsComplexObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new NoDefaultConstructorWithPrivateSetterProperty(42);

		var flat = objectFlattener.Flatten(obj);

		// Act
		var complex = objectFlattener.Unflatten<NoDefaultConstructorWithPrivateSetterProperty>(flat);

		// Assert
		Assert.Equal(obj.Age, complex.Age);
	}

	[Fact]
	public void Unflatten_HasPropertiesWithNoSetter_ReturnsComplexObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new PropertiesWithNoSetter(42, "Username", null);

		var flat = objectFlattener.Flatten(obj);

		// Act
		var complex = objectFlattener.Unflatten<PropertiesWithNoSetter>(flat);

		// Assert
		Assert.Equal(obj.Age, complex.Age);
		Assert.Equal(obj.Name, complex.Name);
		Assert.Equal(obj.Factor, complex.Factor);
	}
}
