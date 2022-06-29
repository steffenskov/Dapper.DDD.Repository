using System.Collections;

namespace Dapper.Repository.Reflection
{
	public class ExtendedPropertyInfoCollection : IReadOnlyExtendedPropertyInfoCollection
	{
		private readonly IDictionary<string, ExtendedPropertyInfo> _dictionary;
		private readonly List<ExtendedPropertyInfo> _list;

		public int Count => _list.Count;

		public ExtendedPropertyInfoCollection()
		{
			_dictionary = new Dictionary<string, ExtendedPropertyInfo>();
			_list = new List<ExtendedPropertyInfo>();
		}

		public ExtendedPropertyInfoCollection(IEnumerable<ExtendedPropertyInfo> properties) : this()
		{
			AddRange(properties);
		}

		public ExtendedPropertyInfo this[int index] => _list[index];
		public ExtendedPropertyInfo this[string propertyName]
		{
			get
			{
				var exist = _dictionary.ContainsKey(propertyName);
				return exist ? _dictionary[propertyName] : throw new InvalidOperationException("Property doesn't exist");
			}
		}

		public bool TryGetValue(string propertyName, out ExtendedPropertyInfo? property)
		{
			return _dictionary.TryGetValue(propertyName, out property);
		}

		public void AddRange(IEnumerable<ExtendedPropertyInfo> properties)
		{
			foreach (var prop in properties)
			{
				_dictionary.Add(prop.Name, prop);
			}

			_list.AddRange(properties);
		}

		public void Add(ExtendedPropertyInfo property)
		{
			_dictionary.Add(property.Name, property);
			_list.Add(property);
		}

		public void Remove(ExtendedPropertyInfo property)
		{
			_ = _dictionary.Remove(property.Name);
			_ = _list.Remove(property);
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
	}
}
