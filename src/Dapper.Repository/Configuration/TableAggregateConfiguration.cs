namespace Dapper.Repository.Configuration;
public class TableAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
{
	public string? TableName { get; set; }

	protected override string EntityName => TableName!;
}
