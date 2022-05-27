using Microsoft.Extensions.Options;

namespace Dapper.Repository.Configuration;

public class SqlAggregateConfiguration<TAggregate> : AggregateConfiguration<TAggregate>
{
	public string? Schema { get; set; }

	public SqlAggregateConfiguration(IOptions<SqlDefaultConfiguration>? options) : base(options)
	{
		Schema = options?.Value.Schema;
	}
}