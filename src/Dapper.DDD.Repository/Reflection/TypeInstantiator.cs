using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.DDD.Repository.Reflection;

internal static class TypeInstantiator
{
	private static readonly LockedConcurrentDictionary<Type, Func<object>> _constructors = new();

	public static T CreateInstance<T>()
	{
		return (T)CreateInstance(typeof(T));
	}

	public static object CreateInstance(Type type)
	{
		if (type.IsValueType)
		{
			return
				Activator.CreateInstance(
					type)
				!; // Value types don't necessarily have a proper default constructor for the IL generation to work, so this is a safe workaround albeit slower than IL.
		}

		var ctor = _constructors.GetOrAdd(type, CreateConstructorDelegate);
		return ctor();
	}

	private static Func<object> CreateConstructorDelegate(Type type)
	{
		var ctorInfo =
			type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
				Type.EmptyTypes, null) ?? ConstructorBuilder.CreateEmptyConstructor(type);

		var ctorDelegate = (Func<object>)CreateDelegate(ctorInfo, typeof(Func<object>));
		return ctorDelegate;
	}

	private static Delegate CreateDelegate(ConstructorInfo constructor, Type delegateType)
	{
		// Create the dynamic method
		var method = new DynamicMethod(GenerateDynamicMethodName(constructor), constructor.DeclaringType, null,
			true);

		// Create the il
		var gen = method.GetILGenerator();
		gen.Emit(OpCodes.Newobj, constructor);
		gen.Emit(OpCodes.Ret);

		return method.CreateDelegate(delegateType);
	}

	private static string GenerateDynamicMethodName(ConstructorInfo constructor)
	{
		return $"{constructor.DeclaringType!.Name}__{Guid.NewGuid().ToString().Replace("-", "")}";
	}
}