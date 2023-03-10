using System.Collections;

namespace Dapper.DDD.Repository;

internal static class Extensions
{
	public static bool IsSimpleOrBuiltIn(this Type type, ISet<Type> treatAsBuiltInTypes)
	{
		return type.IsPrimitive
			   || type.IsEnum
			   || type.Namespace?.StartsWith("System") == true
			   || treatAsBuiltInTypes.Contains(type)
			   || IsNullableSimple(type, treatAsBuiltInTypes);
	}

	private static bool IsNullableSimple(Type type, ISet<Type> treatAsBuiltInTypes)
	{
		return type.IsGenericType
			   && type.IsNullable()
			   && IsSimpleOrBuiltIn(type.GetGenericArguments()[0], treatAsBuiltInTypes);
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