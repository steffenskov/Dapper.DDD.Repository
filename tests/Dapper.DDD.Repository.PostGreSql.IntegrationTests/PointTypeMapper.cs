using System.Data;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PointTypeMapper : SqlMapper.TypeHandler<Point> {
	public override void SetValue(IDbDataParameter parameter, Point value) {
		if (parameter is not NpgsqlParameter npgsqlParameter)
		{
			throw new ArgumentException("parameter is not of type NpgsqlParameter");
		}
		
		npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Geography;
		npgsqlParameter.NpgsqlValue = value;
	}

	public override Point Parse(object value) {
		if (value is not Point point)
		{
			throw new ArgumentException("value is not of type Point");
		}
		
		return point;
	}
}