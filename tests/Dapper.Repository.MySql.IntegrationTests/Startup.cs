namespace Dapper.Repository.MySql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new MySqlConnectionFactory("Server=localhost;Port=33060;Database=northwind;Uid=root;Pwd=mysql1337;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new MySqlQueryGeneratorFactory();
		});
		services.AddTableRepository<Category, int>(options =>
		{
			options.TableName = "categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options => // TODO: how to map TAggregateId?
		{
			options.TableName = "composite_users";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
			options.HasValueObject(x => x.Id);
		});

		services.AddViewRepository<ProductListView, int>(options =>
		{
			options.ViewName = "current_product_list";
			options.HasKey(x => x.ProductID);
		});
		this.Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}