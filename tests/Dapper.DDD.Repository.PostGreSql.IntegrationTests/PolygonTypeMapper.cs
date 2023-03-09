using System.Data;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class PolygonTypeMapper : SqlMapper.TypeHandler<Polygon> {
	public override void SetValue(IDbDataParameter parameter, Polygon value) {
		if (parameter is not NpgsqlParameter npgsqlParameter)
		{
			throw new ArgumentException("parameter is not of type NpgsqlParameter");
		}
		
		npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Geography;
		npgsqlParameter.NpgsqlValue = value;
	}

	public override Polygon Parse(object value) {
		if (value is not Polygon polygon)
		{
			throw new ArgumentException("value is not of type Polygon");
		}
		
		return polygon;
	}
}