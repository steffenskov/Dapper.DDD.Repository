using System.Data;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PolygonTypeMapper : SqlMapper.TypeHandler<Polygon>
{
	public override void SetValue(IDbDataParameter parameter, Polygon value)
	{
		parameter.Value = (object?)value?.AsBinary() ?? DBNull.Value;
		parameter.DbType = DbType.Binary;
	}

	public override Polygon Parse(object? value)
	{
		if (value is null or DBNull)
			return null!;

		var bytes = (byte[])value;
		var reader = new WKBReader();
		return (Polygon)reader.Read(bytes);
	}
}