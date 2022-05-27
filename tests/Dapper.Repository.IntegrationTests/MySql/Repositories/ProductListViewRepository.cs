using System;
using Dapper.Repository.IntegrationTests.Aggregates;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class ProductListViewRepository : MyViewRepository<ProductListViewAggregate>
	{
		protected override string ViewName => "current_product_list";
	}
}
