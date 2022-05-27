using System.Linq.Expressions;

namespace Dapper.Repository.Reflection;
public class ExpressionParser<TAggregate>
{
	public IEnumerable<ExtendedPropertyInfo> GetExtendedPropertiesFromExpression(Expression<Func<TAggregate, object>> expression)
	{
		var propertyNames = GetMemberName(expression);
		var properties = TypePropertiesCache.GetProperties<TAggregate>();
		foreach (var propertyName in propertyNames)
		{
			if (!properties.TryGetValue(propertyName, out var property))
				throw new InvalidOperationException($"{typeof(TAggregate).Name} doesn't contain a property named {propertyName}.");

			yield return property;
		}
	}

	private IEnumerable<string> GetMemberName(Expression expression)
	{
		switch (expression.NodeType)
		{
			case ExpressionType.Lambda:
				return GetMemberName(((LambdaExpression)expression).Body);
			case ExpressionType.MemberAccess:
				return new[] { ((MemberExpression)expression).Member.Name };
			case ExpressionType.Convert:
				return GetMemberName(((UnaryExpression)expression).Operand);
			case ExpressionType.New:
				return ((NewExpression)expression).Members!.Select(m => m.Name);
			default:
				throw new NotSupportedException(expression.NodeType.ToString());
		}
	}
}