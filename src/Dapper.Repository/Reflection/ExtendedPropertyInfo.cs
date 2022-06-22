using System.Reflection;

namespace Dapper.Repository.Reflection;

public class ExtendedPropertyInfo
{
	public PropertyInfo Property { get; }
	public string Name => !string.IsNullOrWhiteSpace(Prefix) ? $"{Prefix}_{Property.Name}" : Property.Name;
	public Type Type => Property.PropertyType;

	//public string Prefix { get; set; } = "";

	public bool HasSetter { get; }

	private readonly object? _defaultValue;
	private readonly MemberAccessor _accessor;

	public ExtendedPropertyInfo(PropertyInfo property)
	{
		Property = property;
		var type = property.PropertyType;

		_accessor = new MemberAccessor(property);
		_defaultValue = TypeDefaultValueCache.GetDefaultValue(type);
		HasSetter = _accessor.HasSetter;
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

	public IReadOnlyList<ExtendedPropertyInfo> GetPropertiesOrdered()
	{
		var result = TypePropertiesCache.GetProperties(Type)
										.Values
										.OrderBy(prop => prop.Name)
										.ToList()
										.AsReadOnly();

		// TODO: Clone property before assigning prefix
		foreach (var prop in result)
		{
			prop.Prefix = Name;
		}

		return result;
	}
}
