using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Dapper.Repository.IntegrationTests
{
	public static class ConnectionFactory
	{

		private const string _mySqlConnectionString = @"Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;";
		private const string _sqlConnectionString = @"Server=localhost;Database=Northwind;User Id=sa;Password=SqlServer2019;";

		public static IDbConnection CreateSqlConnection()
		{
			return new SqlConnection(_sqlConnectionString);
		}

		public static IDbConnection CreateMySqlConnection()
		{
			return new MySqlConnection(_mySqlConnectionString);
		}
	}
}
