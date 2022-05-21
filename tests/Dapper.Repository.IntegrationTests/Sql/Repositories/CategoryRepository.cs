using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class CategoryRepository : MyPrimaryKeyRepository<CategoryPrimaryKeyEntity, CategoryEntity>
	{
		protected override string TableName => "Categories";
	}
}
