using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class CategoryRepository : MyPrimaryKeyRepository<CategoryPrimaryKeyAggregate, Category>
	{
		protected override string TableName => "Categories";
	}
}
