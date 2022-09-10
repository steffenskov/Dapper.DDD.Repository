using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory(
				"Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;AllowPublicKeyRetrieval=true;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveId, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveId, Zipcode.Create);
		});
		services.AddTableRepository<Category, CategoryId>(options =>
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

		services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(
			options =>
			{
				options.ViewName = "current_product_list";
				options.HasKey(x => x.ProductID);
			});
		services.AddViewRepository<ProductListView>(options => { options.ViewName = "current_product_list"; });

		services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "customers_with_value_object";
			options.HasKey(x => x.Id);
			options.Ignore(x => x.IdAndName);
		});

		services.AddTableRepository<CustomerWithNestedAddresses, Guid>(options =>
		{
			options.TableName = "customers_with_nested_value_object";
			options.HasKey(x => x.Id);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}