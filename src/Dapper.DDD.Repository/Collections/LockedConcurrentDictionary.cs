using System.Collections.Concurrent;
namespace Dapper.DDD.Repository.Collections;

/// <summary>
/// Wrapper around ConcurrentDictionary that ensures GetOrAdd only invokes the valueFactory once for a given key by using locks.
/// </summary>
internal class LockedConcurrentDictionary<TKey, TValue>
where TKey : notnull
{
	private readonly ConcurrentDictionary<TKey, TValue> _dictionary = new();
	private readonly object _lock = new();

	/// <summary>
	/// Adds a key/value pair to the Dictionary by using the specified function if the key does not already exist. This is done inside a lock.
	/// Returns the new value, or the existing value if the key exists.
	/// </summary>
	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		if (_dictionary.TryGetValue(key, out var result))
			return result;

		lock (_lock)
		{
			if (_dictionary.TryGetValue(key, out result))
				return result;

			result = valueFactory(key);
			_dictionary[key] = result;
			return result;
		}
	}
}