using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class CompositeUserRepository : MyPrimaryKeyRepository<CompositeUserPrimaryKeyEntity, CompositeUserEntity>
	{
		protected override string TableName => "composite_users";

	}
}
