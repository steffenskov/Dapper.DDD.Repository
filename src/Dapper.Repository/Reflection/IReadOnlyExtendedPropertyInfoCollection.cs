namespace Dapper.Repository.Reflection
{
	public interface IReadOnlyExtendedPropertyInfoCollection
	{
		bool Contains(ExtendedPropertyInfo property);
		IEnumerator<ExtendedPropertyInfo> GetEnumerator();
	}
}