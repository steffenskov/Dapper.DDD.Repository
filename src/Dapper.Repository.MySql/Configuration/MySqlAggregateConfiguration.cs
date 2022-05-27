using Microsoft.Extensions.Options;

namespace Dapper.Repository.Configuration;

public class MySqlAggregateConfiguration<TAggregate> : AggregateConfiguration<TAggregate>
{

	public MySqlAggregateConfiguration(IOptions<MySqlDefaultConfiguration>? options) : base(options)
	{
	}
}