using System.Reflection;

namespace Dapper.DDD.Repository.Reflection;

public class ExtendedPropertyInfo
{
	private readonly MemberAccessor _accessor;

	private readonly object? _defaultValue;

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

	public string Name { get; }
	public Type Type { get; }

	public bool HasSetter => _accessor.HasSetter;

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
	{
		if (aggregate is null)
		{
			return;
		}

		_accessor.SetValue(aggregate, value);
	}

	public IReadOnlyExtendedPropertyInfoCollection GetPropertiesOrdered()
	{
		return new ExtendedPropertyInfoCollection(GetPropertiesOrdered(Type, Name));
	}

	private IOrderedEnumerable<ExtendedPropertyInfo> GetPropertiesOrdered(Type type, string prefix)
	{
		return TypePropertiesCache.GetProperties(type)
			.Select(prop => new ExtendedPropertyInfo(prop, prefix))
			.OrderBy(prop => prop.Name);
	}

	public IReadOnlyExtendedPropertyInfoCollection GetFlattenedPropertiesOrdered<TAggregate>(
		BaseAggregateConfiguration<TAggregate> configuration) where TAggregate : notnull
	{
		return new ExtendedPropertyInfoCollection(GetFlattenedPropertiesOrdered(Type, Name, configuration));
	}

	private IEnumerable<ExtendedPropertyInfo> GetFlattenedPropertiesOrdered<TAggregate>(Type type, string prefix,
		BaseAggregateConfiguration<TAggregate> configuration) where TAggregate : notnull
	{
		var properties = GetPropertiesOrdered(type, prefix);
		foreach (var prop in properties)
		{
			if (prop.Type.IsSimpleOrBuiltIn() || configuration.HasTypeConverter(prop.Type))
			{
				yield return prop;
			}
			else
			{
				foreach (var nestedProp in GetFlattenedPropertiesOrdered(prop.Type, prop.Name, configuration))
				{
					yield return nestedProp;
				}
			}
		}
	}
}