namespace Dapper.DDD.Repository.IntegrationTests.Repositories;

public interface ICustomerRepository : ITableRepository<Customer, Guid>
{
	Task<IEnumerable<Customer>> GetByZipcodeAsync(Zipcode zipcode);
	Task UpdateDeliveryAddress(Guid id, Address newDeliveryAddress);
}
