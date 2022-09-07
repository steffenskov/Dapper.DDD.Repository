using Dapper.DDD.Repository.Configuration;
using Dapper.DDD.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.IntegrationTests.Repositories;

public class CustomerRepository : TableRepository<Customer, Guid>, ICustomerRepository
{
	public CustomerRepository(IOptions<TableAggregateConfiguration<Customer>> options,
		IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}

	public async Task<IEnumerable<Customer>> GetByZipcodeAsync(Zipcode zipcode)
	{
		return await QueryAsync(
			$"SELECT {PropertyList} FROM {TableName} WHERE InvoiceAddress_Zipcode = @zipcode OR DeliveryAddress_Zipcode = @zipcode",
			new { zipcode });
	}

	public async Task UpdateDeliveryAddress(Guid id, Address newDeliveryAddress)
	{
		await ExecuteAsync(
			$"UPDATE {TableName} SET DeliveryAddress_Street = @address_Street, DeliveryAddress_Zipcode = @address_Zipcode WHERE Id = @id",
			new { id, address = newDeliveryAddress });
	}
}