using System.Collections.Generic;
using Dapper.Repository.MetaInformation.PropertyInfos;

namespace Dapper.Repository.MetaInformation
{
	internal record EntityInformation(IReadOnlyCollection<PrimaryKeyPropertyInfo> PrimaryKeys, IReadOnlyCollection<ForeignKeyPropertyInfo> ForeignKeys, IReadOnlyCollection<ColumnPropertyInfo> Columns)
	{
	}
}
