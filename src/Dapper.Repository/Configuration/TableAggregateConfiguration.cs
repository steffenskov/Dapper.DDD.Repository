namespace Dapper.Repository.Configuration;
public class TableAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
{
	public string? TableName { get; set; }

	public override string EntityName => TableName!;
}