using System.Data;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PointTypeMapper : SqlMapper.TypeHandler<Point>
{
	public override void SetValue(IDbDataParameter parameter, Point value)
	{
		parameter.Value = (object?)value?.AsBinary() ?? DBNull.Value;
		parameter.DbType = DbType.Binary;
	}

	public override Point Parse(object? value)
	{
		if (value is null or DBNull)
			return null!;

		return (Point)value;
	}
}