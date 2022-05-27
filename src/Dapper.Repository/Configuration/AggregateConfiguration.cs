using System.Linq.Expressions;
using Dapper.Repository.Reflection;

namespace Dapper.Repository.Configuration;


public class AggregateConfiguration<TAggregate> : IAggregateConfiguration<TAggregate>
{
	private List<ExtendedPropertyInfo>? _keyProperties;
	private readonly List<ExtendedPropertyInfo> _defaults = new();
	private readonly List<ExtendedPropertyInfo> _identities = new();
	private readonly List<ExtendedPropertyInfo> _ignores = new();

	public string? TableName { get; set; }
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	public AggregateConfiguration(IOptions<DefaultConfiguration>? options)
	{
		var defaults = options?.Value;
		QueryGeneratorFactory = defaults?.QueryGeneratorFactory;
		ConnectionFactory = defaults?.ConnectionFactory;
		DapperInjectionFactory = defaults?.DapperInjectionFactory;
	}

	public void HasKey(Expression<Func<TAggregate, object>> expression)
	{
		if (_keyProperties is not null)
			throw new InvalidOperationException("HasKey has already been called once.");

		_keyProperties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression).ToList();
	}

	public void Ignore(Expression<Func<TAggregate, object>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_ignores.AddRange(properties);
	}

	public void HasDefault(Expression<Func<TAggregate, object>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_defaults.AddRange(properties);
	}

	public void HasIdaggregate(Expression<Func<TAggregate, object>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_identities.AddRange(properties);
	}

	public IReadOnlyList<ExtendedPropertyInfo> GetKeys()
	{
		if (_keyProperties is null)
			throw new InvalidOperationException("No key has been specified for this aggregate.");

		return _keyProperties.AsReadOnly();
	}

	public IReadOnlyList<ExtendedPropertyInfo> GetIdaggregateProperties()
	{
		return _identities.AsReadOnly();
	}

	public IReadOnlyList<ExtendedPropertyInfo> GetProperties()
	{
		var rawList = TypePropertiesCache.GetProperties<TAggregate>().Values.ToDictionary(prop => prop.Name); // Clone dictionary so we can mutate it

		foreach (var ignore in _ignores)
		{
			rawList.Remove(ignore.Name);
		}

		return rawList.Values.ToList().AsReadOnly();
	}

	public IReadOnlyList<ExtendedPropertyInfo> GetPropertiesWithDefaultConstraints()
	{
		return _defaults.AsReadOnly();
	}
}
