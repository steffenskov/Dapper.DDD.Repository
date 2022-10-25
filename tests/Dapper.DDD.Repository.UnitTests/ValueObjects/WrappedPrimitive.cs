namespace Dapper.DDD.Repository.UnitTests.ValueObjects;

public readonly struct WrappedPrimitive
{
	private readonly int _value;

	private WrappedPrimitive(int value)
	{
		_value = value;
	}

	public static implicit operator int(WrappedPrimitive wrapped)
	{
		return wrapped._value;
	}

	public static implicit operator WrappedPrimitive(int value)
	{
		return new WrappedPrimitive(value);
	}
}