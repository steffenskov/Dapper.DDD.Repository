using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Dapper.Repository.MetaInformation
{
	internal static class TypeCache
	{
		private static readonly ConcurrentDictionary<Type, IList<PropertyInfo>> _cache;
		static TypeCache()
		{
			_cache = new ConcurrentDictionary<Type, IList<PropertyInfo>>();
		}

		public static IList<PropertyInfo> GetProperties<T>()
		{
			var type = typeof(T);
			if (!_cache.TryGetValue(type, out var result))
			{
				_cache[type] = result = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			}

			return result;
		}
	}
}
