using Dapper.Repository.Configuration;
using Dapper.Repository.IntegrationTests;
using Dapper.Repository.IntegrationTests.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Repository.Sql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new SqlConnectionFactory("Server=localhost;Database=Northwind;User Id=sa;Password=SqlServer2019;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new SqlQueryGeneratorFactory();
			options.Schema = "dbo";
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