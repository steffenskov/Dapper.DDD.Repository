using System;
using System.Reflection;
using Dapper.Repository.Attributes;

namespace Dapper.Repository.MetaInformation.PropertyInfos
{
	internal class ForeignKeyPropertyInfo : ColumnPropertyInfo
	{
		public string ReferencedPropertyName { get; }

		public ForeignKeyPropertyInfo(PropertyInfo property, ForeignKeyColumnAttribute foreignKey)
			: base(property, foreignKey)
		{
			ReferencedPropertyName = foreignKey.ReferencedPropertyName ?? property.Name;
		}
	}
}
