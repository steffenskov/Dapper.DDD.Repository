using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.Repository.Reflection;

/// <summary>
/// Tool to flatten all public properties of an object.
/// </summary>
internal static class ObjectFlattener
{
	private static readonly ConcurrentDictionary<Type, Type> _flatTypeMap = new();
	private static readonly ConcurrentDictionary<Type, bool> _shouldFlattenTypeMap = new();
	private static readonly ModuleBuilder _moduleBuilder;

	static ObjectFlattener()
	{
		var aName = new AssemblyName($"ObjectFlattenerTypes{GenerateStrippedGuid()}");
		var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

		// The module name is usually the same as the assembly name.
		_moduleBuilder = ab.DefineDynamicModule(aName.Name!);
	}

	/// <summary>
	/// Creates an empty flattened object based on the type T.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	public static object FlattenType<T>()
	{
		var type = typeof(T);
		return !ShouldFlattenType(type) ? TypeInstantiator.CreateInstance(type) : CreateFlattenedInstance(type);
	}

	/// <summary>
	/// Creates a flattened object based on the type T with all the properties from the aggregate given.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	/// <param name="aggregate">Aggregate to copy properties from</param>
	public static object Flatten<T>(T aggregate)
	where T : notnull
	{
		var type = typeof(T);
		if (!ShouldFlattenType(type))
		{
			return aggregate;
		}

		var flatResult = CreateFlattenedInstance(type);

		CopyValuesToFlatResult(aggregate, flatResult, flatResult.GetType());

		return flatResult!;
	}

	/// <summary>
	/// Retrieves the type of the flattened version of type T.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	public static Type GetFlattenedType<T>()
	{
		return _flatTypeMap.GetOrAdd(typeof(T), CreateFlattenedType);
	}

	/// <summary>
	/// Unflattens the given flattenedAggregate into its original type T.
	/// </summary>
	/// <typeparam name="T">The original type the flattenedObject is based on</typeparam>
	/// <param name="flattenedObject">A flattened object</param>
	public static T Unflatten<T>(object flattenedObject)
		where T : notnull
	{
		var result = TypeInstantiator.CreateInstance<T>();
		var destinationProperties = TypePropertiesCache.GetProperties<T>();
		var sourceProperties = TypePropertiesCache.GetProperties(flattenedObject.GetType());
		foreach (var sourceProperty in sourceProperties.Values)
		{
			var sourceValue = sourceProperty.GetValue(flattenedObject);
			if (destinationProperties.TryGetValue(sourceProperty.Name, out var destinationProperty))
			{
				destinationProperty.SetValue(result, sourceValue);
			}
			else // We're dealing with a value object which means a nested destination
			{
				CopyValueToNestedDestination(result, sourceProperty.Name, sourceValue);
			}
		}
		return result;
	}

	private static void CopyValueToNestedDestination<T>(T result, string sourcePropertyName, object? sourceValue) where T : notnull
	{
		var parts = sourcePropertyName.Split('_');
		object destinationObject = result;
		var destinationProperties = TypePropertiesCache.GetProperties(destinationObject.GetType());
		foreach (var part in parts[..^1]) // Ensure properties exist for everything up til the one to set the source property on
		{
			var localDestinationProperty = destinationProperties[part];
			var existingValue = localDestinationProperty.GetValue(destinationObject);
			if (existingValue is null)
			{
				existingValue = TypeInstantiator.CreateInstance(localDestinationProperty.Type);
				localDestinationProperty.SetValue(destinationObject, existingValue);
			}
			destinationObject = existingValue;
			destinationProperties = TypePropertiesCache.GetProperties(destinationObject.GetType());
		}
		destinationProperties[parts.Last()].SetValue(destinationObject, sourceValue);
	}

	private static bool ShouldFlattenType(Type type)
	{
		return _shouldFlattenTypeMap.GetOrAdd(type, t =>
		{
			if (t.IsSimpleType())
			{
				return false;
			}

			foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!prop.PropertyType.IsSimpleType())
				{
					return true;
				}
			}
			return false;
		});
	}

	private static void CopyValuesToFlatResult(object aggregate, object flatResult, Type flatType, string prefix = "")
	{
		var destinationProperties = TypePropertiesCache.GetProperties(flatType);
		foreach (var prop in aggregate.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (prop.PropertyType.IsSimpleType())
			{
				destinationProperties[$"{prefix}{prop.Name}"].SetValue(flatResult, prop.GetValue(aggregate));
			}
			else
			{
				var propValue = prop.GetValue(aggregate);
				if (propValue is not null)
				{
					CopyValuesToFlatResult(propValue, flatResult, flatType, $"{prefix}{prop.Name}_");
				}
				else
				{
					destinationProperties[$"{prefix}{prop.Name}"].SetValue(flatResult, prop.GetValue(aggregate));
				}
			}
		}
	}

	private static object CreateFlattenedInstance(Type type)
	{
		var flatType = _flatTypeMap.GetOrAdd(type, CreateFlattenedType);
		return TypeInstantiator.CreateInstance(flatType);
	}

	private static Type CreateFlattenedType(Type type)
	{
		var tb = _moduleBuilder.DefineType($"{type.Name}Flattened{GenerateStrippedGuid()}", TypeAttributes.Public);
		CreateProperties(type, tb);

		return tb.CreateType()!;
	}

	private static void CreateProperties(Type type, TypeBuilder typeBuilder, string prefix = "")
	{
		foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (prop.PropertyType.IsSimpleType())
			{
				CreateProperty(typeBuilder, prefix, prop);
			}
			else
			{
				CreateProperties(prop.PropertyType, typeBuilder, $"{prefix}{prop.Name}_");
			}
		}
	}

	private static void CreateProperty(TypeBuilder typeBuilder, string prefix, PropertyInfo prop)
	{
		var fieldBuilder = typeBuilder.DefineField($"_{prefix}{prop.Name}", prop.PropertyType, FieldAttributes.Private);
		var propertyBuilder = typeBuilder.DefineProperty($"{prefix}{prop.Name}", PropertyAttributes.HasDefault, prop.PropertyType, null);

		var propVisibility = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

		// Define the "get" accessor method for CustomerName.
		var getMethod = typeBuilder.DefineMethod($"get_{prefix}{prop.Name}", propVisibility, prop.PropertyType, Type.EmptyTypes);

		var custNameGetIL = getMethod.GetILGenerator();

		custNameGetIL.Emit(OpCodes.Ldarg_0);
		custNameGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
		custNameGetIL.Emit(OpCodes.Ret);

		// Define the "set" accessor method for CustomerName.
		var setMethod = typeBuilder.DefineMethod($"set_{prefix}{prop.Name}", propVisibility, null, new Type[] { prop.PropertyType });

		var custNameSetIL = setMethod.GetILGenerator();

		custNameSetIL.Emit(OpCodes.Ldarg_0);
		custNameSetIL.Emit(OpCodes.Ldarg_1);
		custNameSetIL.Emit(OpCodes.Stfld, fieldBuilder);
		custNameSetIL.Emit(OpCodes.Ret);

		// Last, we must map the two methods created above to our PropertyBuilder to
		// their corresponding behaviors, "get" and "set" respectively.
		propertyBuilder.SetGetMethod(getMethod);
		propertyBuilder.SetSetMethod(setMethod);
	}

	private static string GenerateStrippedGuid()
	{
		return Guid.NewGuid().ToString().Replace("-", string.Empty);
	}
}
