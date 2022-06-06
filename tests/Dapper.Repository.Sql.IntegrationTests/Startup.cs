using Dapper.Repository.DependencyInjection;

namespace Dapper.Repository.Sql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		_ = services.AddOptions();
		_ = services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new SqlConnectionFactory("Server=127.0.0.1;Database=Northwind;User Id=sa;Password=SqlServer2019;Encrypt=False;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			options.Schema = "dbo";
		});
		_ = services.AddTableRepository<Category, int>(options =>
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
			options.HasValueObject(x => x.Id);
		});

		_ = services.AddTableRepository<Customer, Guid>(options =>
		{
			options.TableName = "CustomersWithValueObject";
			options.HasKey(x => x.Id);
			options.HasValueObject(x => x.Address);
		});

		_ = services.AddViewRepository<ProductListView, int>(options =>
		{
			options.ViewName = "Current Product List";
			options.HasKey(x => x.ProductID);
		});
		Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}
