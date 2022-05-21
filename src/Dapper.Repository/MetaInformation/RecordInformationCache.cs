using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Dapper.Repository.Attributes;
using Dapper.Repository.MetaInformation.PropertyInfos;

namespace Dapper.Repository.MetaInformation
{
	internal static class EntityInformationCache
	{
		private static readonly ConcurrentDictionary<Type, EntityInformation> _cache;
		private static readonly object _cacheLock = new object();

		static EntityInformationCache()
		{
			_cache = new ConcurrentDictionary<Type, EntityInformation>();
		}

		public static EntityInformation GetEntityInformation<TEntity>()
		where TEntity : DbEntity
		{
			var type = typeof(TEntity);
			if (!_cache.TryGetValue(type, out var result))
			{
				lock (_cacheLock)
				{
					if (!_cache.TryGetValue(type, out result))
					{
						_cache[type] = result = CreateEntityInformation<TEntity>();
					}
				}
			}
			return result;
		}

		private static EntityInformation CreateEntityInformation<TEntity>()
		where TEntity : DbEntity
		{
			var properties = TypeCache.GetProperties<TEntity>();
			var primaryKeys = new List<PrimaryKeyPropertyInfo>();
			var foreignKeys = new List<ForeignKeyPropertyInfo>();
			var columns = new List<ColumnPropertyInfo>();

			foreach (var prop in properties)
			{
				var column = prop.GetCustomAttribute<ColumnAttribute>();
				var primaryKey = prop.GetCustomAttribute<PrimaryKeyColumnAttribute>();
				var foreignKey = prop.GetCustomAttribute<ForeignKeyColumnAttribute>();

				if (primaryKey != null)
				{
					primaryKeys.Add(new PrimaryKeyPropertyInfo(prop, primaryKey));
					columns.Add(new ColumnPropertyInfo(prop, primaryKey));
				}
				if (foreignKey != null)
				{
					foreignKeys.Add(new ForeignKeyPropertyInfo(prop, foreignKey));
					columns.Add(new ColumnPropertyInfo(prop, foreignKey));
				}
				if (primaryKey == null && foreignKey == null && column != null)
				{
					columns.Add(new ColumnPropertyInfo(prop, column));
				}
			}

			return new EntityInformation(primaryKeys.AsReadOnly(), foreignKeys.AsReadOnly(), columns.AsReadOnly());
		}
	}
}
