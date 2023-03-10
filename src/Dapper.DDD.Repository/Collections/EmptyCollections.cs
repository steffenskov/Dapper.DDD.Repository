namespace Dapper.DDD.Repository.Collections;

internal static class EmptyCollections
{
	public static ISet<Type> TypeSet { get; } = new HashSet<Type>();
}