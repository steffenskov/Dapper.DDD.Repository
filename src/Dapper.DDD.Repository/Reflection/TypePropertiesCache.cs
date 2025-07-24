using System.Reflection;

namespace Dapper.DDD.Repository.Reflection;

public static class
	TypePropertiesCache // TODO: This really should be internal, how to deal with the QueryGenerators needing this?
{
	private static readonly LockedConcurrentDictionary<Type, IReadOnlyExtendedPropertyInfoCollection> _properties =
		new();

	public static IReadOnlyExtendedPropertyInfoCollection GetNonComputedProperties<T>()
	{
		return GetNonComputedProperties(typeof(T));
	}

	public static IReadOnlyExtendedPropertyInfoCollection GetNonComputedProperties(Type type)
	{
		return _properties.GetOrAdd(type, t =>
		{
			var properties = new ExtendedPropertyInfoCollection();

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				var extendedProperty = new ExtendedPropertyInfo(property);
				if (!extendedProperty.IsComputed)
				{
					properties.Add(extendedProperty);
				}
			}

			return properties;
		});
	}
}