namespace Dapper.DDD.Repository;

internal static class Extensions
{
	public static bool IsSimpleOrBuiltIn(this Type type)
	{
		return type.IsPrimitive
		       || type.IsEnum
		       || type.Namespace?.StartsWith("System") == true
		       || IsNullableSimple(type);
	}

	private static bool IsNullableSimple(Type type)
	{
		return type.IsGenericType
		       && type.GetGenericTypeDefinition() == typeof(Nullable<>)
		       && IsSimpleOrBuiltIn(type.GetGenericArguments()[0]);
	}
}