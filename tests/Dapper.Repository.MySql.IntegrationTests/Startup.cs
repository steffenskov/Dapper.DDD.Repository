namespace Dapper.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory("Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
		});
		services.AddTableRepository<Category, int>(options =>
		{
			options.TableName = "Categories";
			options.HasKey(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options => // TODO: how to map TAggregateId?
		{
			options.TableName = "CompositeUsers";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});

		services.AddViewRepository<ProductListView, int>(options =>
		{
			options.TableName = "Current Product List";
			options.HasKey(x => x.ProductID);
		});
		this.Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}