namespace Dapper.Repository.Reflection;
internal static class TypeInstantiator
{
	public static T New<T>()
	{
		return Activator.CreateInstance<T>(); // TODO: Implement via IL.Emit
	}

	public static object? New(Type type)
	{
		return Activator.CreateInstance(type); // TODO: Implement via IL.Emit
	}
}