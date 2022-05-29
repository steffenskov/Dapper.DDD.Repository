namespace Dapper.Repository.Sql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new SqlConnectionFactory("Server=localhost;Database=Northwind;User Id=sa;Password=SqlServer2019;Encrypt=False;");
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
		services.AddTableRepository<CompositeUser, CompositeUserId>(options => // TODO: how to map TAggregateId?
		{
			options.TableName = "CompositeUsers";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
			options.HasValueObject(x => x.Id);
		});

		services.AddViewRepository<ProductListView, int>(options =>
		{
			options.ViewName = "Current Product List";
			options.HasKey(x => x.ProductID);
		});
		this.Provider = services.BuildServiceProvider();
	}

	public ServiceProvider Provider { get; }
}