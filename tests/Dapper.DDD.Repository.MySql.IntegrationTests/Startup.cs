using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		_ = services.AddOptions();
		_ = services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory("Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;AllowPublicKeyRetrieval=true;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveId, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveId, Zipcode.Create);
		});
		_ = services.AddTableRepository<Category, CategoryId>(options =>
		{
			options.TableName = "categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		_ = services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.TableName = "composite_users";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});

		_ = services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(options =>
		{
			options.ViewName = "current_product_list";
			options.HasKey(x => x.ProductID);
		});
		_ = services.AddViewRepository<ProductListView>(options =>
		{
			options.ViewName = "current_product_list";
		});

		_ = services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "customers_with_value_object";
			options.HasKey(x => x.Id);
			options.Ignore(x => x.IdAndName);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
