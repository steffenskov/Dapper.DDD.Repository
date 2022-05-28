using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Dapper.Repository.Reflection;

public static class TypePropertiesCache // TODO: This really should be internal, how to deal with the QueryGenerators needing this?
{
	private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, ExtendedPropertyInfo>> _properties = new();

	public static IReadOnlyDictionary<string, ExtendedPropertyInfo> GetProperties<T>()
	{
		return GetProperties(typeof(T));
	}

	public static IReadOnlyDictionary<string, ExtendedPropertyInfo> GetProperties(Type type)
	{
		return _properties.GetOrAdd(type, t =>
		{
			var properties = new Dictionary<string, ExtendedPropertyInfo>();

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				properties[property.Name] = new ExtendedPropertyInfo(property);
			}

			return new ReadOnlyDictionary<string, ExtendedPropertyInfo>(properties);
		});
	}
}