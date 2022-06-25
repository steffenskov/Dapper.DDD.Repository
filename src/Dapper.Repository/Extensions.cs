namespace Dapper.Repository;
internal static class Extensions
{
	public static bool IsSimpleType(this Type type)
	{
		return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || IsNullableSimple(type);
	}

	private static bool IsNullableSimple(Type type)
	{
		return type.IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(Nullable<>)
				&& IsSimpleType(type.GetGenericArguments()[0]);
	}
}
