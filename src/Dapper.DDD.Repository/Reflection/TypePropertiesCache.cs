using System.Reflection;

namespace Dapper.DDD.Repository.Reflection;

public static class
	TypePropertiesCache // TODO: This really should be internal, how to deal with the QueryGenerators needing this?
{
	private static readonly LockedConcurrentDictionary<Type, IReadOnlyExtendedPropertyInfoCollection> _properties =
		new();

	public static IReadOnlyExtendedPropertyInfoCollection GetProperties<T>()
	{
		return GetProperties(typeof(T));
	}

	public static IReadOnlyExtendedPropertyInfoCollection GetProperties(Type type)
	{
		return _properties.GetOrAdd(type, t =>
		{
			var properties = new ExtendedPropertyInfoCollection();

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				properties.Add(new ExtendedPropertyInfo(property));
			}

			return properties;
		});
	}
}