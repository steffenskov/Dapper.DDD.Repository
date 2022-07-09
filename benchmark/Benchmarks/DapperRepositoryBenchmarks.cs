using benchmark.Aggregates;
using benchmark.Repositories;
using benchmark.Requirements;
using BenchmarkDotNet.Attributes;
using Dapper;
using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace benchmark.Benchmarks;
public class DapperRepositoryBenchmarks
{
	private CustomerRepository _repo;
	private readonly string _tableName;
	private static object _lock = new();
	private readonly string _connectionString = "Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServerPassword#&%¤2019;Encrypt=False;";

	public DapperRepositoryBenchmarks()
	{
		var tableConfig = new TableAggregateConfiguration<Customer>
		{
			TableName = "CustomersWithValueObject",
			Schema = "dbo"
		};
		tableConfig.HasKey(customer => customer.Id);
		_repo = new CustomerRepository(Options.Create(tableConfig), Options.Create(new DefaultConfiguration
		{
			ConnectionFactory = new SqlConnectionFactory(_connectionString),
			DapperInjectionFactory = new DapperInjectionFactory(),
			QueryGeneratorFactory = new SqlQueryGeneratorFactory()
		}));
		_tableName = _repo.GetTableName();
	}

	public async Task ReseedTable()
	{
		await _repo.SeedTableAsync(1000);
	}

	[Benchmark]
	public async Task Raw_GetAll()
	{
		using var connection = new SqlConnection(_connectionString);
		await connection.QueryAsync<CustomerFlat>($"SELECT * FROM {_tableName}");
	}

	[Benchmark]
	public async Task Repository_GetAll()
	{
		await _repo.GetAllAsync();
	}

	[Benchmark]
	public async Task Raw_Insert()
	{
		using var connection = new SqlConnection(_connectionString);
		var i = Random.Shared.Next();
		var customer = new CustomerFlat
		{
			Id = Guid.NewGuid(),
			Name = $"Customer#{i}",
			DeliveryAddress_Street = $"Street #{i}",
			DeliveryAddress_Zipcode = Random.Shared.Next(),
			InvoiceAddress_Street = $"Street #{i}",
			InvoiceAddress_Zipcode = Random.Shared.Next(),
		};
		await connection.ExecuteAsync($"INSERT INTO {_tableName} (Id, Name, DeliveryAddress_Street, DeliveryAddress_Zipcode, InvoiceAddress_Street, InvoiceAddress_Zipcode) VALUES (@Id, @Name, @DeliveryAddress_Street, @DeliveryAddress_Zipcode, @InvoiceAddress_Street, @InvoiceAddress_Zipcode);", customer);
	}

	[Benchmark]
	public async Task Repository_Insert()
	{
		var i = Random.Shared.Next();
		await _repo.InsertAsync(new Customer
		{

			Id = Guid.NewGuid(),
			Name = $"Customer#{i}",
			DeliveryAddress = new Address
			{
				Street = $"Street #{i}",
				Zipcode = Random.Shared.Next()
			},
			InvoiceAddress = new Address
			{
				Street = $"Street #{i}",
				Zipcode = Random.Shared.Next()
			}
		});
	}

	[Benchmark]
	public async Task Raw_Delete()
	{
		var id = await GetGuidAsync();
		using var connection = new SqlConnection(_connectionString);
		await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id = @Id", new { Id = id });
	}

	[Benchmark]
	public async Task Repository_Delete()
	{
		var id = await GetGuidAsync();
		await _repo.DeleteAsync(id);
	}

	[Benchmark]
	public async Task Raw_Update()
	{
		var id = await GetGuidAsync();
		var customer = new CustomerFlat
		{
			Id = id,
			Name = "Hello world",
			DeliveryAddress_Street = "Some street",
			DeliveryAddress_Zipcode = 42,
			InvoiceAddress_Street = "Some other street",
			InvoiceAddress_Zipcode = 1337
		};
		using var connection = new SqlConnection(_connectionString);
		await connection.ExecuteAsync($@"UPDATE {_tableName} 
SET Name = @Name,
DeliveryAddress_Street = @DeliveryAddress_Street,
DeliveryAddress_Zipcode = @DeliveryAddress_Zipcode,
InvoiceAddress_Street = @InvoiceAddress_Street,
InvoiceAddress_Zipcode = @InvoiceAddress_Zipcode
WHERE Id = @Id", customer);
	}

	[Benchmark]
	public async Task Repository_Update()
	{
		var id = await GetGuidAsync();
		var customer = new Customer
		{
			Id = id,
			Name = "Hello world",
			DeliveryAddress = new Address
			{
				Street = "Some street",
				Zipcode = 42
			},
			InvoiceAddress = new Address
			{
				Street = "Some other street",
				Zipcode = 1337
			}
		};
		await _repo.UpdateAsync(customer);
	}

	private async Task<Guid> GetGuidAsync()
	{
		using var connection = new SqlConnection(_connectionString);
		return await connection.ExecuteScalarAsync<Guid>($"SELECT TOP 1 Id FROM {_tableName}");
	}
}
