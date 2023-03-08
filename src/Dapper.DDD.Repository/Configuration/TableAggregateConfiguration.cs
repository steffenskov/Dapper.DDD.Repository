namespace Dapper.DDD.Repository.Configuration;

public class TableAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
where TAggregate: notnull
{
	public string TableName { get; set; } = default!;

	protected override string EntityName => TableName;
}