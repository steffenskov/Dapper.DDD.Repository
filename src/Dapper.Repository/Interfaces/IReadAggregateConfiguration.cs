using Dapper.Repository.Reflection;

namespace Dapper.Repository.Interfaces;

public interface IReadAggregateConfiguration<TAggregate>
{
	string EntityName { get; }
	IReadOnlyExtendedPropertyInfoCollection GetIdentityProperties();
	IReadOnlyExtendedPropertyInfoCollection GetKeys();
	ExtendedPropertyInfoCollection GetProperties();
	IReadOnlyExtendedPropertyInfoCollection GetPropertiesWithDefaultConstraints();
	IReadOnlyExtendedPropertyInfoCollection GetValueObjects();
}
