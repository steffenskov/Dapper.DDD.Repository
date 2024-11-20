namespace Dapper.DDD.Repository.Configuration;

public class TableAggregateConfiguration<TAggregate> : BaseAggregateConfiguration<TAggregate>
where TAggregate: notnull
{
	public string TableName { get; set; } = default!;

	/// <summary>
	/// Whether the table has triggers, currently only used for MS SQL based databases where it alters DELETE, UPDATE and UPSERT behavior so they don't attempt to read OUTPUT as that is unsupported by MS SQL with triggers.
	/// Defaults to false. 
	/// </summary>
	public bool HasTriggers { get; set; }

	
	protected override string EntityName => TableName;
}