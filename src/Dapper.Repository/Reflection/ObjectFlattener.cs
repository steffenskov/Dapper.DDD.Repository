using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.Repository.Reflection;

internal static class ObjectFlattener
{
	private static ConcurrentDictionary<Type, Type> _flatTypeMap = new();
	private static ConcurrentDictionary<Type, bool> _shouldFlattenTypeMap = new();
	private static ModuleBuilder _moduleBuilder;

	static ObjectFlattener()
	{
		var aName = new AssemblyName("ObjectFlattenerTypes" + Guid.NewGuid());
		var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

		// The module name is usually the same as the assembly name.
		_moduleBuilder = ab.DefineDynamicModule(aName.Name!);
	}

	public static object Flatten<T>(T aggregate)
	where T : notnull
	{
		var type = typeof(T);
		if (!ShouldFlattenType(type))
			return aggregate;

		var flatResult = CreateFlattenedInstance(type);

		CopyValuesToFlatResult(aggregate, flatResult, flatResult.GetType());

		return flatResult!;
	}

	private static bool ShouldFlattenType(Type type)
	{
		return _shouldFlattenTypeMap.GetOrAdd(type, t =>
		{
			if (t.IsSimpleType())
				return false;

			foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!prop.PropertyType.IsSimpleType())
					return true;
			}
			return false;
		});
	}

	private static void CopyValuesToFlatResult(object aggregate, object flatResult, Type targetType, string prefix = "")
	{
		foreach (var prop in aggregate.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (prop.PropertyType.IsSimpleType())
			{
				targetType.GetField($"{prefix}{prop.Name}")!.SetValue(flatResult, prop.GetValue(aggregate)); // Speed up with IL emit?
			}
			else
			{
				var propValue = prop.GetValue(aggregate);
				if (propValue is not null)
					CopyValuesToFlatResult(propValue, flatResult, targetType, $"{prefix}{prop.Name}_");
				else
					targetType.GetField($"{prefix}{prop.Name}")!.SetValue(flatResult, prop.GetValue(aggregate)); // Speed up with IL emit?
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
		var tb = _moduleBuilder.DefineType($"{type.Name}Flattened", TypeAttributes.Public);
		CreateFields(type, tb);

		return tb.CreateType()!;
	}

	private static void CreateFields(Type type, TypeBuilder tb, string prefix = "")
	{
		foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (prop.PropertyType.IsSimpleType())
			{
				tb.DefineField($"{prefix}{prop.Name}", prop.PropertyType, FieldAttributes.Public);
			}
			else
			{
				CreateFields(prop.PropertyType, tb, $"{prefix}{prop.Name}_");
			}
		}
	}
}
