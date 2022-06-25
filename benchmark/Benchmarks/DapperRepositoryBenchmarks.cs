using benchmark.Aggregates;
using benchmark.Repositories;
using benchmark.Requirements;
using BenchmarkDotNet.Attributes;
using Dapper.Repository.Configuration;
using Dapper.Repository.Sql;
using Microsoft.Extensions.Options;

namespace benchmark.Benchmarks
{
	public class DapperRepositoryBenchmarks
	{
		private CustomerRepository _repo;

		public DapperRepositoryBenchmarks()
		{
			var tableConfig = new TableAggregateConfiguration<Customer>
			{
				TableName = "Customers",
				Schema = "dbo"
			};
			tableConfig.HasKey(customer => customer.CustomerID);
			_repo = new CustomerRepository(Options.Create(tableConfig), Options.Create(new DefaultConfiguration
			{
				ConnectionFactory = new SqlConnectionFactory("Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServer2019;Encrypt=False;"),
				DapperInjectionFactory = new DapperInjectionFactory(),
				QueryGeneratorFactory = new SqlQueryGeneratorFactory()
			}));
		}

		[Benchmark]
		public async Task GetAllUsingRepositoryAsync()
		{
			await _repo.GetAllAsync();
		}
	}
}
