using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.IntegrationTests.Repositories;
using NetTopologySuite.Geometries;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests;

public class Startup
{
	public Startup()
	{
		var services = new ServiceCollection();
		services.AddOptions();
		SqlMapper.AddTypeHandler(new PolygonTypeMapper());
		SqlMapper.AddTypeHandler(new PointTypeMapper());
		services.ConfigureDapperRepositoryDefaults(options =>
		{
			options.ConnectionFactory = new PostGreSqlConnectionFactory(
				"Server=localhost;Port=55432;Database=northwind;Uid=postgres;Pwd=postgres;");
			options.DapperInjectionFactory = new DapperInjectionFactory();
			options.QueryGeneratorFactory = new PostGreSqlQueryGeneratorFactory();
			options.AddTypeConverter<CategoryId, int>(categoryId => categoryId.PrimitiveId, CategoryId.Create);
			options.AddTypeConverter<Zipcode, int>(zipcode => zipcode.PrimitiveId, Zipcode.Create);
			options.AddTypeConverter<Polygon, Polygon>(val => val, val => val);
			options.AddTypeConverter<Point, Point>(val => val, val => val);
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

	public ServiceProvider Provider { get; }
}