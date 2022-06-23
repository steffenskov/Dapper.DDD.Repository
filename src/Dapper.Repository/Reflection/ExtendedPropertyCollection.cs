using System.Collections;

namespace Dapper.Repository.Reflection
{
	public class ExtendedPropertyInfoCollection : IEnumerable<ExtendedPropertyInfo>, IReadOnlyExtendedPropertyInfoCollection
	{
		private readonly IDictionary<string, ExtendedPropertyInfo> _dictionary;
		private readonly IList<ExtendedPropertyInfo> _list;

		public ExtendedPropertyInfoCollection()
		{
			_dictionary = new Dictionary<string, ExtendedPropertyInfo>();
			_list = new List<ExtendedPropertyInfo>();
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
