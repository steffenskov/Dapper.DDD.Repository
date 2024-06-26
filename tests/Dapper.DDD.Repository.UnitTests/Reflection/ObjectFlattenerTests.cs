﻿using Dapper.DDD.Repository.Reflection;
using Dapper.DDD.Repository.UnitTests.Aggregates;
using Dapper.DDD.Repository.UnitTests.ValueObjects;

namespace Dapper.DDD.Repository.UnitTests.Reflection;

public class ObjectFlattenerTests
{
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
			Id = Guid.NewGuid(), Name = "Test", Address = new { Street = "Test street", City = "Test city" }
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
		objectFlattener.AddTypeConverter(typeof(StrongUserId),
			new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid)));

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() =>
			objectFlattener.AddTypeConverter(typeof(StrongUserId),
				new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid))));

		Assert.Equal($"A TypeConverter for the type {typeof(StrongUserId)} has already been added.", ex.Message);
	}

	[Fact]
	public void Flatten_HasCustomType_ReturnsFlatObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new UserWithStrongTypedId { Id = new StrongUserId(42), Username = "Some name" };

		objectFlattener.AddTypeConverter(typeof(StrongUserId),
			new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid)));

		// Act
		var result = objectFlattener.Flatten(obj);

		var resultType = result.GetType();

		object getResultValue(string name)
		{
			return resultType!.GetProperty(name)!.GetValue(result)!;
		}

		// Assert
		Assert.NotSame(obj, result);
		Assert.Equal(obj.Id.PrimitiveValue, getResultValue("Id"));
		Assert.Equal(obj.Username, getResultValue("Username"));
	}

	[Fact]
	public void Flatten_HasCustomTypeInValueObject_ReturnsFlatObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new CustomerWithUser
		{
			Id = 42, User = new UserValueObject { Id = new StrongUserId(1337), Username = "Some user" }
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId),
			new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid)));

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
		var obj = new UserWithStrongTypedId { Id = new StrongUserId(42), Username = "Some name" };

		objectFlattener.AddTypeConverter(typeof(StrongUserId),
			new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid)));
		var flat = objectFlattener.Flatten(obj);

		// Act
		var complex = objectFlattener.Unflatten<UserWithStrongTypedId>(flat);

		// Assert
		Assert.NotSame(obj, complex);
		Assert.Equal(obj.Id.PrimitiveValue, complex.Id!.PrimitiveValue);
		Assert.Equal(obj.Username, complex.Username);
	}

	[Fact]
	public void Unflatten_HasCustomTypeInValueObject_ReturnsComplexObject()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var obj = new CustomerWithUser
		{
			Id = 42, User = new UserValueObject { Id = new StrongUserId(1337), Username = "Some user" }
		};

		objectFlattener.AddTypeConverter(typeof(StrongUserId),
			new TypeConverter<StrongUserId, int>(uid => uid.PrimitiveValue, uid => new StrongUserId(uid)));
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
	
	[Fact]
	public void Flatten_HasNullableWrappedPrimitiveWithValueWithTypeConverter_Works()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var converter = new TypeConverter<WrappedPrimitive, int>(val => val, val => val);
		objectFlattener.AddTypeConverter(typeof(WrappedPrimitive), converter);
		var aggregate = new AggregateWithWrappedPrimitive()
		{
			Wrapped = 42,
			NullableWrapped = 1337
		};
		
		// Act
		var flattened = objectFlattener.Flatten(aggregate);
		var unflattened = objectFlattener.Unflatten<AggregateWithWrappedPrimitive>(flattened);
		
		// Assert
		Assert.Equal(42, (int)unflattened.Wrapped);
		Assert.Equal(1337, (int)unflattened.NullableWrapped!.Value);
	}

	[Fact]
	public void Flatten_HasNullWrappedPrimitiveWithTypeConverter_Works()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		var converter = new TypeConverter<WrappedPrimitive, int>(val => val, val => val);
		objectFlattener.AddTypeConverter(typeof(WrappedPrimitive), converter);
		var aggregate = new AggregateWithWrappedPrimitive()
		{
			Wrapped = 42
		};

		// Act
		var flattened = objectFlattener.Flatten(aggregate);
		var unflattened = objectFlattener.Unflatten<AggregateWithWrappedPrimitive>(flattened);

		// Assert
		Assert.Equal(42, (int)unflattened.Wrapped);
		Assert.Null(unflattened.NullableWrapped);
	}

	[Fact]
	public void Flatten_HasNullableWrappedGenericPrimitiveWithValueWithTypeConverter_Works()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		objectFlattener.AddTypeConverter(typeof(WrappedGenericPrimitive<double>),
			new TypeConverter<WrappedGenericPrimitive<double>, double>(val => val, val => val));
		objectFlattener.AddTypeConverter(typeof(WrappedGenericPrimitive<Guid>),
			new TypeConverter<WrappedGenericPrimitive<Guid>, Guid>(val => val, val => val));

		var guid = Guid.NewGuid();
		var aggregate = new AggregateWithWrappedGenericPrimitive()
		{
			Double=4.2,
			Guid =guid
		};

		// Act
		var flattened = objectFlattener.Flatten(aggregate);
		var unflattened = objectFlattener.Unflatten<AggregateWithWrappedGenericPrimitive>(flattened);

		// Assert
		Assert.Equal(4.2, (double)unflattened.Double);
		Assert.Equal(guid, (Guid)unflattened.Guid!.Value);
	}

	[Fact]
	public void Flatten_HasNullWrappedGenericPrimitiveWithTypeConverter_Works()
	{
		// Arrange
		var objectFlattener = new ObjectFlattener();
		objectFlattener.AddTypeConverter(typeof(WrappedGenericPrimitive<double>),
			new TypeConverter<WrappedGenericPrimitive<double>, double>(val => val, val => val));
		objectFlattener.AddTypeConverter(typeof(WrappedGenericPrimitive<Guid>),
			new TypeConverter<WrappedGenericPrimitive<Guid>, Guid>(val => val, val => val));
		var aggregate = new AggregateWithWrappedGenericPrimitive()
		{
			Double = 4.2
		};

		// Act
		var flattened = objectFlattener.Flatten(aggregate);
		var unflattened = objectFlattener.Unflatten<AggregateWithWrappedGenericPrimitive>(flattened);

		// Assert
		Assert.Equal(4.2, (double)unflattened.Double);
		Assert.Null(unflattened.Guid);
	}

	public class NoDefaultConstructorWithPrivateSetterProperty
	{
		public NoDefaultConstructorWithPrivateSetterProperty(int age)
		{
			Age = age;
		}

		public int Age { get; }
	}

	public class PropertiesWithNoSetter
	{
		public PropertiesWithNoSetter(int age, string name, double? factor)
		{
			Age = age;
			Name = name;
			Factor = factor;
		}

		public int Age { get; }
		public string Name { get; }
		public double? Factor { get; }
	}
}