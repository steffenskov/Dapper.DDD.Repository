using System;
using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class ProductListViewRepository : MyViewRepository<ProductListViewAggregate>
	{
		protected override string ViewName => "Current Product List";
	}
}
