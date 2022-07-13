using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		_ = services.AddOptions();
		_ = services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new SqlConnectionFactory("Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServerPassword#&%¤2019;Encrypt=False;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			options.Schema = "dbo";
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveId, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveId, Zipcode.Create);

		});
		_ = services.AddTableRepository<Category, CategoryId>(options =>
		{
			options.TableName = "Categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		_ = services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.TableName = "CompositeUsers";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});

		_ = services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(options =>
		{
			options.ViewName = "[Current Product List]";
			options.HasKey(x => x.ProductID);
		});
		_ = services.AddViewRepository<ProductListView>(options =>
		{
			options.ViewName = "[Current Product List]";
		});

		_ = services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "CustomersWithValueObject";
			options.HasKey(x => x.Id);
			options.Ignore(x => x.IdAndName);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
