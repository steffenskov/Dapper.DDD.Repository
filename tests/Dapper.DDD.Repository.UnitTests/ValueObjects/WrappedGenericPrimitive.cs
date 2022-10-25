namespace Dapper.DDD.Repository.UnitTests.ValueObjects;

public readonly struct WrappedGenericPrimitive<T>
where T: struct
{
	private readonly T _value;

	private WrappedGenericPrimitive(T value)
	{
		_value = value;
	}

	public static implicit operator T(WrappedGenericPrimitive<T> wrapped)
	{
		return wrapped._value;
	}

	public static implicit operator WrappedGenericPrimitive<T>(T value)
	{
		return new WrappedGenericPrimitive<T>(value);
	}
}