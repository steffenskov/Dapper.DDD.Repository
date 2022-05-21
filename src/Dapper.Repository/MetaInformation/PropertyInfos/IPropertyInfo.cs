using System;
using System.Reflection;

namespace Dapper.Repository.MetaInformation.PropertyInfos
{
	internal interface IPropertyInfo
	{
		PropertyInfo Property { get; }
		string Name { get; }
		Type Type { get; }
	}
}
