using System.Linq.Expressions;
using Dapper.Repository.Reflection;

namespace Dapper.Repository.Configuration;

public abstract class BaseAggregateConfiguration<TAggregate> : IReadAggregateConfiguration<TAggregate>
{
	private List<ExtendedPropertyInfo>? _keyProperties;
	private readonly List<ExtendedPropertyInfo> _defaults = new();
	private readonly List<ExtendedPropertyInfo> _identities = new();
	private readonly List<ExtendedPropertyInfo> _ignores = new();
	private readonly List<ExtendedPropertyInfo> _valueObjects = new();

	public string? Schema { get; set; }
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	public abstract string EntityName { get; }

	internal void SetDefaults(DefaultConfiguration? defaults)
	{
		if (defaults is null)
			return;
		Schema ??= defaults.Schema;
		QueryGeneratorFactory ??= defaults.QueryGeneratorFactory;
		ConnectionFactory ??= defaults.ConnectionFactory;
		DapperInjectionFactory ??= defaults.DapperInjectionFactory;
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

	public void HasIdentity(Expression<Func<TAggregate, object>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_identities.AddRange(properties);
	}

	public void HasValueObject(Expression<Func<TAggregate, object>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);
		var invalidProperties = properties.Where(property => property.Type.IsPrimitive || property.Type == typeof(string) || property.Type == typeof(Guid));
		if (invalidProperties.Any())
			throw new ArgumentException($"The properties {string.Join(", ", invalidProperties.Select(p => p.Name))} are not value objects.");

		_valueObjects.AddRange(properties);
	}

	IReadOnlyList<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetKeys()
	{
		if (_keyProperties is null)
			throw new InvalidOperationException("No key has been specified for this aggregate.");

		return _keyProperties.AsReadOnly();
	}

	IReadOnlyList<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetIdentityProperties()
	{
		return _identities.AsReadOnly();
	}

	IReadOnlyList<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetProperties()
	{
		var rawList = TypePropertiesCache.GetProperties<TAggregate>().Values.ToDictionary(prop => prop.Name); // Clone dictionary so we can mutate it

		foreach (var ignore in _ignores)
		{
			rawList.Remove(ignore.Name);
		}

		return rawList.Values.ToList().AsReadOnly();
	}

	IReadOnlyList<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetPropertiesWithDefaultConstraints()
	{
		return _defaults.AsReadOnly();
	}

	IReadOnlyList<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetValueObjects()
	{
		return _valueObjects.AsReadOnly();
	}
}
