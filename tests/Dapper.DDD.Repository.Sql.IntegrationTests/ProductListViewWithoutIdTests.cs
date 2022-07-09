namespace Dapper.DDD.Repository.Sql.IntegrationTests;
public class ProductListViewWithoutIdTests : BaseProductListViewWithoutIdTests, IClassFixture<Startup>
{
	public ProductListViewWithoutIdTests(Startup startup) : base(startup.Provider)
	{
	}
}
