using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class CompositeUserRepository : MyPrimaryKeyRepository<CompositeUserPrimaryKeyEntity, CompositeUserEntity>
	{
		protected override string TableName => "CompositeUsers";

	}
}
