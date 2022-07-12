using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class BaseCustomRepositoryTests
{
	private readonly IServiceProvider _serviceProvider;

	public BaseCustomRepositoryTests(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	[Fact]
	public void GetService_RepositoryConfigured_RepositoryIsRetrieved()
	{
		// Act
		var repository = _serviceProvider.GetService<ICustomerRepository>();

		// Assert
		Assert.NotNull(repository);
		Assert.True(repository is ITableRepository<Customer, Guid>);
	}

	[Fact]
	public async Task GetByCustomtype_AggregateExist_IsRetrieved()
	{
		// Arrange
		var repository = _serviceProvider.GetService<ICustomerRepository>()!;
		await repository.InsertAsync(new Customer
		{
			DeliveryAddress = new Address
			{
				Street = "Some road",
				Zipcode = new Zipcode(1234)
			},
			InvoiceAddress = new Address
			{
				Street = "Some other street",
				Zipcode = new Zipcode(5678)
			},
			Id = Guid.NewGuid(),
			Name = "My customer name"
		});

		// Act
		var fetched = await repository.GetByZipcodeAsync(new Zipcode(1234));

		// Assert
		Assert.NotEmpty(fetched);
	}

	[Fact]
	public async Task CustomUpdateWithComplexType_IsValid_IsUpdated()
	{
		// Arrange
		var repository = _serviceProvider.GetService<ICustomerRepository>()!;
		var inserted = await repository.InsertAsync(new Customer
		{
			DeliveryAddress = new Address
			{
				Street = "Some road",
				Zipcode = new Zipcode(1234)
			},
			InvoiceAddress = new Address
			{
				Street = "Some other street",
				Zipcode = new Zipcode(5678)
			},
			Id = Guid.NewGuid(),
			Name = "My customer name"
		});

		// Act
		await repository.UpdateDeliveryAddress(inserted.Id, new Address { Street = "A brand new road", Zipcode = new Zipcode(9999) });
		var fetched = await repository.GetAsync(inserted.Id);

		// Assert
		Assert.Equal("A brand new road", fetched!.DeliveryAddress.Street);
		Assert.Equal(new Zipcode(9999), fetched.DeliveryAddress.Zipcode);
	}
}
