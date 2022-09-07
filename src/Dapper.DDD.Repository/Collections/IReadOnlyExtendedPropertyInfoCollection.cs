using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Collections;

public interface IReadOnlyExtendedPropertyInfoCollection : IEnumerable<ExtendedPropertyInfo>
{
	int Count { get; }
	ExtendedPropertyInfo this[int index] { get; }
	ExtendedPropertyInfo this[string propertyName] { get; }

	bool Contains(ExtendedPropertyInfo property);

	bool TryGetValue(string propertyName, out ExtendedPropertyInfo? property);
}