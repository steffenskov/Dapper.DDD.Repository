using System.Linq.Expressions;

namespace Dapper.Repository.Reflection;
internal class ExpressionParser<TAggregate>
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

	private static IEnumerable<string> GetMemberName(Expression expression)
	{
		return expression.NodeType switch
		{
			ExpressionType.Lambda => GetMemberName(((LambdaExpression)expression).Body),
			ExpressionType.MemberAccess => new[] { ((MemberExpression)expression).Member.Name },
			ExpressionType.Convert => GetMemberName(((UnaryExpression)expression).Operand),
			ExpressionType.New => ((NewExpression)expression).Members!.Select(m => m.Name),
			_ => throw new NotSupportedException($"Unsupported node type: {expression.NodeType}")
		};
	}
}