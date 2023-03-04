using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new PostGreSqlConnectionFactory(
				"Server=localhost;Port=55432;Database=northwind;Uid=postgres;Pwd=postgres;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new PostGreSqlQueryGeneratorFactory();
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
		services.AddViewRepository<string, IInvalidQueryRepository, InvalidQueryRepository>(options =>
		{
			options.ViewName = "InvalidView";
		});
		services.AddViewRepository<DummyAggregate, int>(options =>
		{
			options.ViewName = "DummyView";
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}