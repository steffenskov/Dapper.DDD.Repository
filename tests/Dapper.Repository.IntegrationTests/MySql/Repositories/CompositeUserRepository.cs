using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class CompositeUserRepository : MyPrimaryKeyRepository<CompositeUserPrimaryKeyAggregate, CompositeUserAggregate>
	{
		protected override string TableName => "composite_users";

	}
}
