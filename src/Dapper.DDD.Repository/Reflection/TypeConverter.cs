namespace Dapper.DDD.Repository.Reflection;

internal interface ITypeConverter
{
	object ConvertToSimple(object source);
	object ConvertToComplex(object source);
	Type SimpleType { get; }
}

internal class TypeConverter<TComplexType, TSimpleType> : ITypeConverter
	where TComplexType : notnull
	where TSimpleType : notnull
{
	private readonly Func<TComplexType, TSimpleType> _convertToSimple;
	private readonly Func<TSimpleType, TComplexType> _convertToComplex;

	public Type SimpleType
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

	public TypeConverter(Func<TComplexType, TSimpleType> convertToSimple, Func<TSimpleType, TComplexType> convertToComplex)
	{
		_convertToSimple = convertToSimple;
		_convertToComplex = convertToComplex;
	}

	public object ConvertToSimple(object source)
	{
		return _convertToSimple((TComplexType)source);
	}

	public object ConvertToComplex(object source)
	{
		return _convertToComplex((TSimpleType)source);
	}
}
