namespace Dapper.Repository.Reflection
{
	public interface IReadOnlyExtendedPropertyInfoCollection : IEnumerable<ExtendedPropertyInfo>
	{
		int Count { get; }
		ExtendedPropertyInfo this[int index] { get; }
		ExtendedPropertyInfo this[string propertyName] { get; }

		bool Contains(ExtendedPropertyInfo property);

		bool TryGetValue(string propertyName, out ExtendedPropertyInfo? property);
	}
}
