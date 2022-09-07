namespace Dapper.DDD.Repository.Configuration;

public class ViewAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
{
	public string ViewName { get; set; } = default!;

	protected override string EntityName => ViewName;
}