using System.Reflection;
using DotNet.Testcontainers.Builders;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Testcontainers.MsSql;

namespace Dapper.DDD.Repository.Sql.IntegrationTests.Configuration;

public class ContainerFixture : IAsyncLifetime, IContainerFixture
{
	private MsSqlContainer? _container;
	
	public async Task InitializeAsync()
	{
		var connectionFactory = await InitializeTestContainerAsync();
		
		var services = new ServiceCollection();
		services.AddOptions();
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = connectionFactory;
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory =
				new SqlQueryGeneratorFactory().SerializeColumnType(type =>
					type.Namespace == "NetTopologySuite.Geometries");
			options.Schema = "dbo";
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveValue, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveValue, Zipcode.Create);
			options.AddTypeConverter<Point, byte[]>(
				geo => new SqlServerBytesWriter { IsGeography = false }.Write(geo),
				bytes => (Point)new SqlServerBytesReader { IsGeography = false }.Read(bytes));
			options.AddTypeConverter<Polygon, byte[]>(
				geo => new SqlServerBytesWriter { IsGeography = false }.Write(geo),
				bytes => (Polygon)new SqlServerBytesReader { IsGeography = false }.Read(bytes));
		});
		services.AddTableRepository<Category, CategoryId>(options =>
		{
			options.TableName = "Categories";
			options.HasKey(x => x.CategoryID);
			options.HasIdentity(x => x.CategoryID);
		});
		services.AddTableRepository<CompositeUser, CompositeUserId>(options =>
		{
			options.TableName = "CompositeUsers";
			options.HasKey(x => x.Id);
			options.HasDefault(x => x.DateCreated);
		});
		
		services.AddViewRepository<ProductListView, int, IProductListViewRepository, ProductListViewRepository>(
			options =>
			{
				options.ViewName = "[Current Product List]";
				options.HasKey(x => x.ProductID);
			});
		services.AddViewRepository<ProductListView>(options => { options.ViewName = "[Current Product List]"; });
		
		services.AddTableRepository<Customer, Guid, ICustomerRepository, CustomerRepository>(options =>
		{
			options.TableName = "CustomersWithValueObject";
			options.HasKey(x => x.Id);
			options.Ignore(x => x.IdAndName);
		});
		services.AddTableRepository<City, Guid>(options =>
		{
			options.TableName = "Cities";
			options.HasKey(x => x.Id);
		});
		services.AddTableRepository<CustomerWithNestedAddresses, Guid>(options =>
		{
			options.TableName = "CustomersWithNestedValueObject";
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
	
	private async Task<SqlConnectionFactory> InitializeTestContainerAsync()
	{
		_container = new MsSqlBuilder()
			.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
			.WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
			.Build();
		
		var startTask = _container.StartAsync();
		
		var northwindScript = await GetNorthwindScriptAsync();
		await startTask;
		var connectionString = _container.GetConnectionString();
		
		await using var sqlConnection = new SqlConnection(connectionString);
		var svrConnection = new ServerConnection(sqlConnection);
		var server = new Server(svrConnection);
		server.ConnectionContext.ExecuteNonQuery(northwindScript);
		
		return new SqlConnectionFactory(connectionString);
	}
	
	private static async Task<string> GetNorthwindScriptAsync()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var resourceName = @"Dapper.DDD.Repository.Sql.IntegrationTests.Resources.northwind.sql";
		
		await using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream is null)
		{
			throw new InvalidOperationException("Couldn't open northwind.sql");
		}
		
		using var reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}
}