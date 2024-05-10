using System.Data;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PolygonTypeMapper : SqlMapper.TypeHandler<Polygon>
{
	public override void SetValue(IDbDataParameter parameter, Polygon? value)
	{
		parameter.Value = (object?)value?.AsBinary() ?? DBNull.Value;
		parameter.DbType = DbType.Binary;
	}
	
	public override Polygon Parse(object? value)
	{
		if (value is null or DBNull)
		{
			return null!;
		}
		
		return (Polygon)value;
	}
}