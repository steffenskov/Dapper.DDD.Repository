using System.Collections.Concurrent;

namespace Dapper.Repository.Reflection;
internal static class TypeDefaultValueCache
{
	private static readonly ConcurrentDictionary<Type, object?> _defaultValues = new();

	public static object? GetDefaultValue(Type type)
	{
		if (!type.IsValueType)
			return null;

		return _defaultValues.GetOrAdd(type, type => TypeInstantiator.New(type));
	}
}