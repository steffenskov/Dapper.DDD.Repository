using benchmark.Aggregates;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;

namespace benchmark.Benchmarks
{
	public class RawDapperBenchmarks
	{
		private string _connectionString;

		public RawDapperBenchmarks()
		{
			_connectionString = "Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServer2019;Encrypt=False;";
		}

		[Benchmark]
		public async Task GetAllUsingRawDapperAsync()
		{
			var connection = new SqlConnection(_connectionString);
			await connection.QueryAsync<Customer>("SELECT * FROM Customers");
		}
	}
}
