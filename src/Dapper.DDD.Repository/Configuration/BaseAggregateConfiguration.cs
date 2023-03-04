using System.Linq.Expressions;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Configuration;

public abstract class BaseAggregateConfiguration<TAggregate> : IReadAggregateConfiguration<TAggregate>
{
	private readonly ExtendedPropertyInfoCollection _defaults = new();
	private readonly ExtendedPropertyInfoCollection _identities = new();
	private readonly ExtendedPropertyInfoCollection _ignores = new();
	private readonly IDictionary<string, string> _columnNameMap = new Dictionary<string, string>();
	private DefaultConfiguration? _defaultConfiguration;
	private ExtendedPropertyInfoCollection? _keyProperties;

	public string? Schema { get; set; }
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	protected abstract string EntityName { get; }

	string IReadAggregateConfiguration<TAggregate>.EntityName => EntityName;

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetKeys()
	{
		return _keyProperties ?? new ExtendedPropertyInfoCollection();
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetIdentityProperties()
	{
		return _identities;
	}

	ExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.GetProperties()
	{
		var rawList =
			TypePropertiesCache.GetProperties<TAggregate>()
				.ToDictionary(prop => prop.Name); // Clone dictionary so we can mutate it

		foreach (var ignore in _ignores)
		{
			_ = rawList.Remove(ignore.Name);
		}

		return new ExtendedPropertyInfoCollection(rawList.Values);
	}

	IReadOnlyExtendedPropertyInfoCollection IReadAggregateConfiguration<TAggregate>.
		GetPropertiesWithDefaultConstraints()
	{
		return _defaults;
	}

	IEnumerable<ExtendedPropertyInfo> IReadAggregateConfiguration<TAggregate>.GetValueObjects()
	{
		var ignoredNames = _ignores.Select(prop => prop.Name).ToHashSet();
		return TypePropertiesCache.GetProperties<TAggregate>()
			.Where(prop => !ignoredNames.Contains(prop.Name))
			.Where(prop => !prop.Type.IsSimpleOrBuiltIn() && !HasTypeConverter(prop.Type));
	}

	public bool HasTypeConverter(Type type)
	{
		return _defaultConfiguration?.HasTypeConverter(type) == true;
	}

	internal void SetDefaults(DefaultConfiguration? defaults)
	{
		if (defaults is null)
		{
			return;
		}

		_defaultConfiguration = defaults;

		Schema ??= defaults.Schema;
		QueryGeneratorFactory ??= defaults.QueryGeneratorFactory;
		ConnectionFactory ??= defaults.ConnectionFactory;
		DapperInjectionFactory ??= defaults.DapperInjectionFactory;
	}

	public void HasColumnName(Expression<Func<TAggregate, object?>> expression, string columnName)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression).ToList();
		if (properties.Count != 1)
		{
			throw new InvalidOperationException("The expression given to HasColumnName must return exactly one property.");
		}
		var property = properties[0];
		if (_columnNameMap.ContainsKey(property.Name))
		{
			throw new InvalidOperationException($@"HasColumnName has already been called once for the property ""{property.Name}"".");
		}

		_columnNameMap[property.Name] = columnName;
	}

	public void HasDefault(Expression<Func<TAggregate, object?>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_defaults.AddRange(properties);
	}

	public void HasIdentity(Expression<Func<TAggregate, object?>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_identities.AddRange(properties);
	}

	public void HasKey(Expression<Func<TAggregate, object?>> expression)
	{
		if (_keyProperties is not null)
		{
			throw new InvalidOperationException("HasKey has already been called once.");
		}

		_keyProperties =
			new ExtendedPropertyInfoCollection(
				new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression));
	}

	public void Ignore(Expression<Func<TAggregate, object?>> expression)
	{
		var properties = new ExpressionParser<TAggregate>().GetExtendedPropertiesFromExpression(expression);

		_ignores.AddRange(properties);
	}
}