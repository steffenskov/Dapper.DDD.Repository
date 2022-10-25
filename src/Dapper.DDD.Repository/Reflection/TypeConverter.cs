namespace Dapper.DDD.Repository.Reflection;

internal interface ITypeConverter
{
	Type SimpleTypeWithNullableSupport { get; }
	Type SimpleTypeRaw { get; }
	object ConvertToSimple(object source);
	object ConvertToComplex(object source);
}

internal class TypeConverter<TComplexType, TSimpleType> : ITypeConverter
	where TComplexType : notnull
	where TSimpleType : notnull
{
	private readonly Func<TSimpleType, TComplexType> _convertToComplex;
	private readonly Func<TComplexType, TSimpleType> _convertToSimple;

	public TypeConverter(Func<TComplexType, TSimpleType> convertToSimple,
		Func<TSimpleType, TComplexType> convertToComplex)
	{
		_convertToSimple = convertToSimple;
		_convertToComplex = convertToComplex;
	}

	public Type SimpleTypeWithNullableSupport
	{
		get
		{
			var simpleType = typeof(TSimpleType);
			if (simpleType.IsValueType) // Wrap in nullable to support nulls
			{
				simpleType = typeof(Nullable<>).MakeGenericType(simpleType);
			}

			return simpleType;
		}
	}

	public Type SimpleTypeRaw => typeof(TSimpleType);

	public object ConvertToSimple(object source)
	{
		return _convertToSimple((TComplexType)source);
	}

	public object ConvertToComplex(object source)
	{
		return _convertToComplex((TSimpleType)source);
	}
}