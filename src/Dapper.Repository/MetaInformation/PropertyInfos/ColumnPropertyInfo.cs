using System;
using System.Reflection;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.MetaInformation.PropertyInfos
{
	internal class ColumnPropertyInfo : IPropertyInfo
	{
		public string ColumnName { get; }
		public bool IsCustomColumnName { get; }
		public bool HasDefaultConstraint { get; }
		public PropertyInfo Property { get; }
		public string Name => Property.Name;
		public Type Type => Property.PropertyType;

		public bool HasSetter { get; }

		private readonly object? _defaultValue;
		private readonly MemberAccessor _accessor;

		public ColumnPropertyInfo(PropertyInfo property, ColumnAttribute column)
		{
			Property = property;
			ColumnName = column.ColumnName ?? property.Name;
			IsCustomColumnName = column.ColumnName != null;
			HasDefaultConstraint = column.HasDefaultConstraint;

			var type = property.PropertyType;

			_accessor = new MemberAccessor(property);
			if (type.IsValueType)
			{
				_defaultValue = ValueTypeDefaultCache.GetDefaultValue(type);
			}
			else
			{
				_defaultValue = null;
			}
			HasSetter = _accessor.HasSetter;
		}

		public bool HasDefaultValue<T>(T entity) where T : DbEntity
		{
			var value = GetValue(entity);

			return value == _defaultValue || value?.Equals(_defaultValue) == true;
		}

		public object GetValue<T>(T entity) where T : DbEntity
		{
			return _accessor.getter(entity);
		}
	}
}
