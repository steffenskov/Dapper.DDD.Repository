using Dapper.DDD.Repository.Exceptions;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

public class AggregateWithValueObjectAsyncTests : BaseAggregateWithValueObjectAsyncTests<DapperRepositoryQueryException>, IClassFixture<Startup>
{
	public AggregateWithValueObjectAsyncTests(Startup startup) : base(startup.Provider)
	{
	}
}
