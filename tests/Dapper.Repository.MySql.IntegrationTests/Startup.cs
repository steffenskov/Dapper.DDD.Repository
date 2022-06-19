using Dapper.Repository.DependencyInjection;
using Dapper.Repository.IntegrationTests.Repositories;

namespace Dapper.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		_ = services.AddOptions();
		_ = services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory("Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
		});
		_ = services.AddTableRepository<Category, int>(options =>
		{
			options.TableName = "categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		_ = services.AddTableRepository<CompositeUser, CompositeUserId>(options => // TODO: how to map TAggregateId?
		{
			options.TableName = "composite_users";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
			options.HasValueObject(x => x.Id);
		});

		_ = services.AddViewRepository<ProductListView, int>(options =>
		{
			options.ViewName = "current_product_list";
			options.HasKey(x => x.ProductID);
		});

		_ = services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "customers_with_value_object";
			options.HasKey(x => x.Id);
			options.HasValueObject(x => x.Address);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
