using Dapper.Repository.DependencyInjection;
using Dapper.Repository.IntegrationTests.Repositories;

namespace Dapper.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory("Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;AllowPublicKeyRetrieval=true;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
		});
		services.AddTableRepository<Category, int>(options =>
		{
			options.TableName = "categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.TableName = "composite_users";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});

		services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(options =>
		{
			options.ViewName = "current_product_list";
			options.HasKey(x => x.ProductID);
		});
		services.AddViewRepository<ProductListView>(options =>
		{
			options.ViewName = "current_product_list";
		});

		services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "customers_with_value_object";
			options.HasKey(x => x.Id);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
