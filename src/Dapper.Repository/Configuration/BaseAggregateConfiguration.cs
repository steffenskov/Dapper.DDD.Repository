using System.Linq.Expressions;
using Dapper.Repository.Reflection;

namespace Dapper.Repository.Configuration;

public abstract class BaseAggregateConfiguration<TAggregate> : IReadAggregateConfiguration<TAggregate>
{
	private ExtendedPropertyInfoCollection? _keyProperties;
	private readonly ExtendedPropertyInfoCollection _defaults = new();
	private readonly ExtendedPropertyInfoCollection _identities = new();
	private readonly ExtendedPropertyInfoCollection _ignores = new();
	private readonly ExtendedPropertyInfoCollection _valueObjects = new();

	public string? Schema { get; set; }
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	protected abstract string EntityName { get; }

	internal void SetDefaults(DefaultConfiguration? defaults)
	{
		if (defaults is null)
		{
			return;
		}

		Schema ??= defaults.Schema;
		QueryGeneratorFactory ??= defaults.QueryGeneratorFactory;
		ConnectionFactory ??= defaults.ConnectionFactory;
		DapperInjectionFactory ??= defaults.DapperInjectionFactory;
	}

	public void HasKey(Expression<Func<TAggregate, object>> expression)
	{
		if (_keyProperties is not null)
		{
			throw new InvalidOperationException("HasKey has already been called once.");
		}

		_keyProperties = new(new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression));
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
		var invalidProperties = properties.Where(property => property.Type.IsSimpleType());
		if (invalidProperties.Any())
		{
			throw new ArgumentException($"The properties {string.Join(", ", invalidProperties.Select(p => p.Name))} are not value objects.");
		}

		_valueObjects.AddRange(properties);
	}

	string IReadAggregateConfiguration<TAggregate>.EntityName => EntityName;

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetKeys()
	{
		return _keyProperties ?? throw new InvalidOperationException("No key has been specified for this aggregate.");
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetIdentityProperties()
	{
		return _identities;
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetProperties()
	{
		var rawList = TypePropertiesCache.GetProperties<TAggregate>().ToDictionary(prop => prop.Name); // Clone dictionary so we can mutate it

		foreach (var ignore in _ignores)
		{
			_ = rawList.Remove(ignore.Name);
		}

		return new ExtendedPropertyInfoCollection(rawList.Values);
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetPropertiesWithDefaultConstraints()
	{
		return _defaults;
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetValueObjects()
	{
		return _valueObjects;
	}
}
