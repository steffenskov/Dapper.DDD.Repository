namespace Dapper.Repository.Configuration;
public class ViewAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
{
	public string? ViewName { get; set; }

	protected override string EntityName => ViewName!;
}
