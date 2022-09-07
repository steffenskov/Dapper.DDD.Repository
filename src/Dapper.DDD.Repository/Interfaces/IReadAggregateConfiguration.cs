using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Interfaces;

public interface IReadAggregateConfiguration<TAggregate>
{
	string EntityName { get; }
	IReadOnlyExtendedPropertyInfoCollection GetIdentityProperties();
	IReadOnlyExtendedPropertyInfoCollection GetKeys();
	ExtendedPropertyInfoCollection GetProperties();
	IReadOnlyExtendedPropertyInfoCollection GetPropertiesWithDefaultConstraints();
	IEnumerable<ExtendedPropertyInfo> GetValueObjects();
	bool HasTypeConverter(Type type);
}