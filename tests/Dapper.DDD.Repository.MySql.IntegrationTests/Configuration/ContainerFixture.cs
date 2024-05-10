using System.Reflection;
using Testcontainers.MySql;

namespace Dapper.DDD.Repository.MySql.IntegrationTests.Configuration;

public class ContainerFixture : IAsyncLifetime, IContainerFixture
{
	private MySqlContainer? _container;
	
	public async Task InitializeAsync()
	{
		var connectionFactory = await InitializeTestContainerAsync();
		
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = connectionFactory;
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
		services.AddViewRepository<string, IInvalidQueryRepository, InvalidQueryRepository>(options => { options.ViewName = "InvalidView"; });
		services.AddViewRepository<DummyAggregate, int>(options => { options.ViewName = "DummyView"; });
		Provider = services.BuildServiceProvider();
	}
	
	public async Task DisposeAsync()
	{
		if (_container is not null)
		{
			await _container.DisposeAsync();
		}
	}
	
	public ServiceProvider Provider { get; private set; } = default!;
	
	private async Task<MySqlConnectionFactory> InitializeTestContainerAsync()
	{
		_container = new MySqlBuilder()
			.WithDatabase("northwind")
			.Build();
		
		var startTask = _container.StartAsync();
		
		var createTableScript = await GetNorthwindScriptAsync("CreateTable");
		var insertDataScript = await GetNorthwindScriptAsync("InsertData");
		await startTask;
		var connectionString = $"{_container.GetConnectionString()};Allow User Variables=True";
		var connectionFactory = new MySqlConnectionFactory(connectionString);
		
		using var connection = connectionFactory.CreateConnection();
		await connection.ExecuteAsync(createTableScript);
		await connection.ExecuteAsync(insertDataScript);
		return connectionFactory;
	}
	
	private static async Task<string> GetNorthwindScriptAsync(string filename)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var resourceName = $"Dapper.DDD.Repository.MySql.IntegrationTests.Resources.{filename}.sql";
		
		await using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream is null)
		{
			throw new InvalidOperationException($"Couldn't open {filename}.sql");
		}
		
		using var reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}
}