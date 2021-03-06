using System.Reflection;

namespace Dapper.DDD.Repository.Reflection;

public class ExtendedPropertyInfo
{
	public string Name { get; }
	public Type Type { get; }

	public bool HasSetter => _accessor.HasSetter;

	private readonly object? _defaultValue;
	private readonly MemberAccessor _accessor;

	public ExtendedPropertyInfo(PropertyInfo property)
	{
		Type = property.PropertyType;
		Name = property.Name;
		_accessor = new MemberAccessor(property);
		_defaultValue = TypeDefaultValueCache.GetDefaultValue(Type);
	}

	private ExtendedPropertyInfo(ExtendedPropertyInfo property, string prefix)
	{
		Type = property.Type;
		Name = $"{prefix}_{property.Name}";
		_accessor = property._accessor;
		_defaultValue = property._defaultValue;
	}

	public bool HasDefaultValue<T>(T aggregate)
	where T : notnull
	{
		var value = GetValue(aggregate);

		return value == _defaultValue || value?.Equals(_defaultValue) == true;
	}

	public object? GetValue<T>(T aggregate)
	where T : notnull
	{
		return _accessor.GetValue(aggregate);
	}

	public void SetValue<T>(T aggregate, object? value)
	where T : notnull
	{
		_accessor.SetValue(aggregate, value);
	}

	public IReadOnlyExtendedPropertyInfoCollection GetPropertiesOrdered()
	{
		return new ExtendedPropertyInfoCollection(
					TypePropertiesCache.GetProperties(Type)
					.Select(prop => new ExtendedPropertyInfo(prop, Name))
					.OrderBy(prop => prop.Name));
	}
}
