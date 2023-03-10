using System.Collections;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Collections;

public class ExtendedPropertyInfoCollection : IReadOnlyExtendedPropertyInfoCollection
{
	private readonly IDictionary<string, ExtendedPropertyInfo> _dictionary;
	private readonly List<ExtendedPropertyInfo> _list;

	public ExtendedPropertyInfoCollection()
	{
		_dictionary = new Dictionary<string, ExtendedPropertyInfo>();
		_list = new List<ExtendedPropertyInfo>();
	}

	public ExtendedPropertyInfoCollection(IEnumerable<ExtendedPropertyInfo> properties) : this()
	{
		AddRange(properties);
	}

	public int Count => _list.Count;

	public ExtendedPropertyInfo this[int index] => _list[index];
	public ExtendedPropertyInfo this[string propertyName] => _dictionary[propertyName];

	public bool TryGetValue(string propertyName, out ExtendedPropertyInfo? property)
	{
		return _dictionary.TryGetValue(propertyName, out property);
	}

	public bool Contains(ExtendedPropertyInfo property)
	{
		return _dictionary.ContainsKey(property.Name);
	}

	public IEnumerator<ExtendedPropertyInfo> GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void AddRange(IEnumerable<ExtendedPropertyInfo> properties)
	{
		if (properties is not ICollection<ExtendedPropertyInfo> collection)
		{
			collection = properties.ToList();
		}
		var existingKeys = collection.Where(prop => _dictionary.ContainsKey(prop.Name));
		if (existingKeys.Any())
		{
			throw new ArgumentException("One or more items with the same key has already been added.");
		}
		foreach (var prop in collection)
		{
			_dictionary.Add(prop.Name, prop);
		}

		_list.AddRange(collection);
	}

	public void Add(ExtendedPropertyInfo property)
	{
		_dictionary.Add(property.Name, property);
		_list.Add(property);
	}

	public void Remove(ExtendedPropertyInfo property)
	{
		if (!_dictionary.TryGetValue(property.Name, out var actualProperty))
		{
			return;
		}
		_dictionary.Remove(property.Name);
		_list.Remove(actualProperty);
	}
}