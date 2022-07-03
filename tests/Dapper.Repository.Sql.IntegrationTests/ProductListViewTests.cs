namespace Dapper.Repository.Sql.IntegrationTests;
public class ProductListViewTests : BaseProductListViewTests, IClassFixture<Startup>
{
	public ProductListViewTests(Startup startup) : base(startup.Provider)
	{
	}
}
