using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class CompositeUserRepository : MyPrimaryKeyRepository<CompositeUserPrimaryKeyAggregate, CompositeUserAggregate>
	{
		protected override string TableName => "CompositeUsers";

	}
}
