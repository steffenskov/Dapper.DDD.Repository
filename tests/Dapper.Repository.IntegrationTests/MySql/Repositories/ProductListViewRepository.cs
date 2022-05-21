using System;
using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.MySql.Repositories
{
	public class ProductListViewRepository : MyViewRepository<ProductListViewEntity>
	{
		protected override string ViewName => "current_product_list";
	}
}
