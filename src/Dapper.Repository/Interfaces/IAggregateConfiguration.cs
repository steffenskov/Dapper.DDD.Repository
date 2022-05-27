using System.Linq.Expressions;

namespace Dapper.Repository.Interfaces;

public interface IAggregateConfiguration<TAggregate>
{
	string? Schema { get; set; }
	string? TableName { get; set; }
	IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	IConnectionFactory? ConnectionFactory { get; set; }
	IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	void HasDefault(Expression<Func<TAggregate, object>> expression);
	void HasIdentity(Expression<Func<TAggregate, object>> expression);
	void HasKey(Expression<Func<TAggregate, object>> expression);
	void Ignore(Expression<Func<TAggregate, object>> expression);
}