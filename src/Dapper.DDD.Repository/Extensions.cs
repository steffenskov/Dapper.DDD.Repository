using System.Collections;

namespace Dapper.DDD.Repository;

internal static class Extensions
{
	public static bool IsSimpleOrBuiltIn(this Type type, ISet<Type> treatAsSimpleTypes)
	{
		return type.IsPrimitive
			   || type.IsEnum
			   || type.Namespace?.StartsWith("System") == true
			   || treatAsSimpleTypes.Contains(type)
			   || IsNullableSimple(type, treatAsSimpleTypes);
	}

	private static bool IsNullableSimple(Type type, ISet<Type> treatAsSimpleTypes)
	{
		return type.IsGenericType
			   && type.IsNullable()
			   && IsSimpleOrBuiltIn(type.GetGenericArguments()[0], treatAsSimpleTypes);
	}

	public static bool IsNullable(this Type type)
	{
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
	}

	public static bool IsGenericEnumerable(this Type type)
	{
		if (!type.IsGenericType)
		{
			return false;
		}

		return type.IsAssignableTo(typeof(IEnumerable));
	}
}