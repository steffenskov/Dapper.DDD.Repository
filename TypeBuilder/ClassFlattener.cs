using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace TypeBuilderDemo;

public class ObjectFlattener
{
	private static ConcurrentDictionary<Type, Func<object>> _flatTypeConstructors = new();
	private static ModuleBuilder _moduleBuilder;

	static ObjectFlattener()
	{
		var aName = new AssemblyName("ObjectFlattenerTypes");
		var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

		// The module name is usually the same as the assembly name.
		_moduleBuilder = ab.DefineDynamicModule(aName.Name!);
	}

	public object Flatten<T>(T aggregate)
	where T : notnull
	{
		var type = typeof(T);
		if (type.IsSimpleType())
			return aggregate;

		var flatResult = CreateFlattenedInstance(type);

		CopyValuesToFlatResult(aggregate, flatResult, flatResult.GetType());

		return flatResult!;
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
		return _flatTypeConstructors.GetOrAdd(type, CreateFlattenedTypeConstructor).Invoke();
	}

	private static Func<object> CreateFlattenedTypeConstructor(Type complexType)
	{
		var flatType = CreateFlattenedType(complexType);

		Func<object> ctor = () => Activator.CreateInstance(flatType)!; // TODO: Create and emit IL for ctor
		return ctor;
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