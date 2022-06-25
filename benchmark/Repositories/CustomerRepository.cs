using benchmark.Aggregates;
using Dapper.Repository.Configuration;
using Dapper.Repository.Interfaces;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace benchmark.Repositories;
public class CustomerRepository : TableRepository<Customer, Guid>, ITableRepository<Customer, Guid>
{
	public string GetTableName() => this.TableName;

	public CustomerRepository(IOptions<TableAggregateConfiguration<Customer>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task SeedTableAsync(int recordCount)
	{
		await ExecuteAsync($"DELETE FROM {TableName}");
		for (var j = 0; j < 10; j++)
		{
			for (var i = 0; i < recordCount / 10; i++)
			{
				await this.InsertAsync(new Customer
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
			System.Console.WriteLine($"{(j + 1) * 10}% seeded");
		}
	}
}
