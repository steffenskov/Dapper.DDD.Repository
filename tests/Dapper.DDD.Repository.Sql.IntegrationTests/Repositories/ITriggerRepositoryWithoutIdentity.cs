using Dapper.DDD.Repository.Configuration;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.Sql.IntegrationTests.Repositories;

public interface ITriggerRepositoryWithoutIdentity : ITableRepository<TriggerEntityWithoutIdentity, int>
{
}

public class TriggerRepositoryWithoutIdentity : TableRepository<TriggerEntityWithoutIdentity, int>, ITriggerRepositoryWithoutIdentity
{
	public TriggerRepositoryWithoutIdentity(IOptions<TableAggregateConfiguration<TriggerEntityWithoutIdentity>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}