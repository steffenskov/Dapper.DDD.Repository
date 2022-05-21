using System;
using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests.Sql.Repositories
{
	public class ProductListViewRepository : MyViewRepository<ProductListViewEntity>
	{
		protected override string ViewName => "Current Product List";
	}
}
