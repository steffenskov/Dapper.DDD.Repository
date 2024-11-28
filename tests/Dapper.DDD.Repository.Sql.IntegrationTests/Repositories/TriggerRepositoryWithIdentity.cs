using Dapper.DDD.Repository.Configuration;
using Microsoft.Extensions.Options;

namespace Dapper.DDD.Repository.Sql.IntegrationTests.Repositories;

public interface ITriggerRepositoryWithIdentity : ITableRepository<TriggerEntityWithIdentity, int>
{
}

public class TriggerRepositoryWithIdentity : TableRepository<TriggerEntityWithIdentity, int>, ITriggerRepositoryWithIdentity
{
	public TriggerRepositoryWithIdentity(IOptions<TableAggregateConfiguration<TriggerEntityWithIdentity>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}