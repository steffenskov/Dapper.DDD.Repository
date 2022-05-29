using Dapper.Repository.Reflection;

namespace Dapper.Repository.Interfaces;

public interface IReadAggregateConfiguration<TAggregate>
{
	string EntityName { get; }
	IReadOnlyList<ExtendedPropertyInfo> GetIdentityProperties();
	IReadOnlyList<ExtendedPropertyInfo> GetKeys();
	IReadOnlyList<ExtendedPropertyInfo> GetProperties();
	IReadOnlyList<ExtendedPropertyInfo> GetPropertiesWithDefaultConstraints();
	IReadOnlyList<ExtendedPropertyInfo> GetValueObjects();
}
