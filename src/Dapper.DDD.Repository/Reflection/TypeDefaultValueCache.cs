namespace Dapper.DDD.Repository.Reflection;

internal static class TypeDefaultValueCache
{
	private static readonly LockedConcurrentDictionary<Type, object?> _defaultValues = new();

	public static object? GetDefaultValue(Type type)
	{
		return !type.IsValueType ? null : _defaultValues.GetOrAdd(type, TypeInstantiator.CreateInstance);
	}
}