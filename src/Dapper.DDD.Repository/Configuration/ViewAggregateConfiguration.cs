namespace Dapper.DDD.Repository.Configuration;

public class ViewAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
	where TAggregate: notnull
{
	public string ViewName { get; set; } = default!;

	protected override string EntityName => ViewName;
}