using System.Linq.Expressions;

namespace Dapper.Repository.Interfaces;

public interface IAggregateConfiguration<TAggregate>
{
	string? TableName { get; set; }
	IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	IConnectionFactory? ConnectionFactory { get; set; }
	IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	void HasDefault(Expression<Func<TAggregate, object>> expression);
	void HasIdaggregate(Expression<Func<TAggregate, object>> expression);
	void HasKey(Expression<Func<TAggregate, object>> expression);
	void Ignore(Expression<Func<TAggregate, object>> expression);
}