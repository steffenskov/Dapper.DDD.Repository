using System.Collections.Concurrent;
using Dapper.DDD.Repository.QueryGenerators;
using Dapper.DDD.Repository.Reflection;

namespace Dapper.DDD.Repository.Configuration;

public class DefaultConfiguration
{
	internal readonly ConcurrentDictionary<Type, ITypeConverter> _typeConverters = new();
	internal readonly ISet<Type> _treatAsSimpleType = new HashSet<Type>();
	public string? Schema { get; set; }
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }

	public void AddTypeConverter<TComplex, TSimple>(Func<TComplex, TSimple> convertToSimple,
		Func<TSimple, TComplex> convertToComplex)
		where TComplex : notnull
		where TSimple : notnull
	{
		if (!_typeConverters.TryAdd(typeof(TComplex),
			    new TypeConverter<TComplex, TSimple>(convertToSimple, convertToComplex)))
		{
			throw new InvalidOperationException(
				$"A TypeConverter has already been added of this type: <{typeof(TComplex)},{typeof(TSimple)}>");
		}
	}

	public void AddTypeConverter<TComplex, TSimple>(
		(Func<TComplex, TSimple> convertToSimple, Func<TSimple, TComplex> convertToComplex) converters)
		where TComplex : notnull
		where TSimple : notnull
	{
		if (!_typeConverters.TryAdd(typeof(TComplex),
			    new TypeConverter<TComplex, TSimple>(converters.convertToSimple, converters.convertToComplex)))
		{
			throw new InvalidOperationException(
				$"A TypeConverter has already been added of this type: <{typeof(TComplex)},{typeof(TSimple)}>");
		}
	}

	public void TreatAsSimpleType<T>()
	{
		_treatAsSimpleType.Add(typeof(T));
	}

	public bool HasTypeConverter(Type type)
	{
		return _typeConverters.ContainsKey(type);
	}
}