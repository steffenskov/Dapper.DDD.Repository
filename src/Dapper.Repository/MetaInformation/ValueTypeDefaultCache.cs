using System;
using System.Collections.Concurrent;

namespace Dapper.Repository.MetaInformation
{
	internal static class ValueTypeDefaultCache
	{
		private static readonly ConcurrentDictionary<Type, object> _defaultValues;

		static ValueTypeDefaultCache()
		{
			_defaultValues = new ConcurrentDictionary<Type, object>();
		}

		public static object GetDefaultValue(Type type)
		{
			if (!_defaultValues.TryGetValue(type, out var result))
			{
				_defaultValues[type] = result = Activator.CreateInstance(type)!;
			}
			return result;
		}
	}
}