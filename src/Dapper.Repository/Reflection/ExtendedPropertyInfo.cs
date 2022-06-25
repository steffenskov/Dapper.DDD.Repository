using System.Reflection;

namespace Dapper.Repository.Reflection;

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
		return _accessor._getter(aggregate);
	}

	public void SetValue<T>(T aggregate, object? value)
	where T : notnull
	{
		_accessor._setter(aggregate, value);
	}

	public IReadOnlyExtendedPropertyInfoCollection GetPropertiesOrdered()
	{
		return new ExtendedPropertyInfoCollection(
					TypePropertiesCache.GetProperties(Type)
					.Values
					.Select(prop => new ExtendedPropertyInfo(prop, Name))
					.OrderBy(prop => prop.Name));
	}
}
