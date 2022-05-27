using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class CategoryRepository : MyPrimaryKeyRepository<CategoryPrimaryKeyAggregate, Category>
	{
		protected override string TableName => "categories";
	}
}
