using Dapper.DDD.Repository.IntegrationTests.Configuration;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests.Configuration;

public class ContainerFixture : IAsyncLifetime, IContainerFixture
{
	private PostgreSqlContainer? _container;
	
	public async Task InitializeAsync()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		
		SqlMapper.AddTypeHandler(new PointTypeMapper());
		SqlMapper.AddTypeHandler(new PolygonTypeMapper());
		
		var connectionFactory = await InitializeTestContainerAsync();
		
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = connectionFactory;
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new PostGreSqlQueryGeneratorFactory();
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveId, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveId, Zipcode.Create);
			options.TreatAsBuiltInType<Polygon>(); // Necessary to allow the SqlMapper to work its magic
			options.TreatAsBuiltInType<Point>(); // Necessary to allow the SqlMapper to work its magic
		});
		services.AddTableRepository<Category, CategoryId>(options =>
		{
			options.Schema = "public";
			options.TableName = "categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.Schema = "public";
			options.TableName = "composite_users";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});
		
		services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(
			options =>
			{
				options.Schema = "public";
				options.ViewName = "current_product_list";
				options.HasKey(x => x.ProductID);
			});
		services.AddViewRepository<ProductListView>(options => { options.ViewName = "current_product_list"; });
		
		services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.Schema = "public";
			options.TableName = "customers_with_value_object";
			options.HasKey(x => x.Id);
			options.Ignore(x => x.IdAndName);
		});
		
		services.AddTableRepository<CustomerWithNestedAddresses, Guid>(options =>
		{
			options.Schema = "public";
			options.TableName = "customers_with_nested_value_object";
			options.HasKey(x => x.Id);
		});
		services.AddTableRepository<City, Guid>(options =>
		{
			options.Schema = "public";
			options.TableName = "cities";
			options.HasKey(x => x.Id);
		});
		services.AddViewRepository<string, IInvalidQueryRepository, InvalidQueryRepository>(options =>
		{
			options.Schema = "public";
			options.ViewName = "InvalidView";
		});
		services.AddViewRepository<DummyAggregate, int>(options =>
		{
			options.Schema = "public";
			options.ViewName = "DummyView";
		});
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
	
	private async Task<PostGreSqlConnectionFactory> InitializeTestContainerAsync()
	{
		_container = new PostgreSqlBuilder()
			.WithImage("postgis/postgis:latest")
			.WithDatabase("northwind")
			.Build();
		
		var startTask = _container.StartAsync();
		
		var northwindScript = await GetNorthwindScriptAsync();
		await startTask;
		var connectionFactory = new PostGreSqlConnectionFactory(_container.GetConnectionString());
		
		using var connection = connectionFactory.CreateConnection();
		await connection.ExecuteAsync(northwindScript);
		return connectionFactory;
	}
	
	private static async Task<string> GetNorthwindScriptAsync()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var resourceName = @"Dapper.DDD.Repository.PostGreSql.IntegrationTests.Resources.pgsql_northwind.sql";
		
		await using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream is null)
		{
			throw new InvalidOperationException("Couldn't open pgsql_northwind.sql");
		}
		
		using var reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}
}