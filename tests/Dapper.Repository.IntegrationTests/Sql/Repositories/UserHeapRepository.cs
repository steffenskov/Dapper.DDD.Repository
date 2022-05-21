using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class UserHeapRepository : MyHeapRepository<UserHeapEntity>
	{
		protected override string TableName => "Heaps";
	}
}