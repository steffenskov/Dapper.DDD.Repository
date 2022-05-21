using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class UserHeapRepository : MyHeapRepository<UserHeapEntity>
	{
		protected override string TableName => "heaps";
	}
}