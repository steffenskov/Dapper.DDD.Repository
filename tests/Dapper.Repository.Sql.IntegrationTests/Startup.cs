using Dapper.Repository.DependencyInjection;
using Dapper.Repository.IntegrationTests.Repositories;

namespace Dapper.Repository.Sql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new SqlConnectionFactory("Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServerPassword#&%¤2019;Encrypt=False;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			options.Schema = "dbo";
		});
		services.AddTableRepository<Category, int>(options =>
		{
			options.TableName = "Categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.TableName = "CompositeUsers";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
			options.HasValueObject(x => x.Id);
		});

		services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(options =>
		{
			options.ViewName = "[Current Product List]";
			options.HasKey(x => x.ProductID);
		});
		services.AddViewRepository<ProductListView>(options =>
		{
			options.ViewName = "[Current Product List]";
		});

		services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "CustomersWithValueObject";
			options.HasKey(x => x.Id);
			options.HasValueObject(x => x.InvoiceAddress);
			options.HasValueObject(x => x.DeliveryAddress);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
