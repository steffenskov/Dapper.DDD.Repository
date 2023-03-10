using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.DDD.Repository.Reflection;

/// <summary>
///     Tool to flatten all public properties of an object.
/// </summary>
internal class ObjectFlattener
{
	private static readonly LockedConcurrentDictionary<Type, Type> _flatTypeMap = new();
	private static readonly LockedConcurrentDictionary<Type, bool> _shouldFlattenTypeMap = new();

	private static readonly ConcurrentDictionary<Type, IReadOnlyExtendedPropertyInfoCollection> _typeProperties =
		new();

	private static readonly ModuleBuilder _moduleBuilder;
	private readonly ConcurrentDictionary<Type, ITypeConverter> _typeConverters = new();
	private readonly ISet<Type> _treatAsBuiltInType = new HashSet<Type>();

	static ObjectFlattener()
	{
		var aName = new AssemblyName($"ObjectFlattenerTypes{GenerateStrippedGuid()}");
		var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

		// The module name is usually the same as the assembly name.
		_moduleBuilder = ab.DefineDynamicModule(aName.Name!);
	}

	public static void SetTypeProperties(Type type, IReadOnlyExtendedPropertyInfoCollection properties)
	{
		_typeProperties.TryAdd(type, properties); // Ignore if it already exists as it would be the same collection
	}

	public void AddTypeConverter(Type type, ITypeConverter converter)
	{
		if (!_typeConverters.TryAdd(type, converter))
		{
			throw new InvalidOperationException($"A TypeConverter for the type {type} has already been added.");
		}
	}

	public void TreatAsBuiltInType(Type type)
	{
		_treatAsBuiltInType.Add(type);
	}

	public ISet<Type> GetTreatAsBuiltInTypes()
	{
		return _treatAsBuiltInType;
	}

	/// <summary>
	///     Creates an empty flattened object based on the type T.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	public object FlattenType<T>()
	{
		var type = typeof(T);
		return !ShouldFlattenType(type)
			? TypeInstantiator.CreateInstance(type)
			: CreateFlattenedInstance(type);
	}

	internal bool TryGetTypeConverter(Type type, out ITypeConverter? converter)
	{
		return _typeConverters.TryGetValue(type, out converter);
	}

	/// <summary>
	///     Creates a flattened object based on the type T with all the properties from the aggregate given.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	/// <param name="aggregate">Aggregate to copy properties from</param>
	public object Flatten<T>(T aggregate)
		where T : notnull
	{
		var type = typeof(T);
		if (!ShouldFlattenType(type))
		{
			return aggregate;
		}

		var flatResult = CreateFlattenedInstance(type);

		CopyValuesToFlatResult(aggregate, flatResult, flatResult.GetType());

		return flatResult!;
	}

	/// <summary>
	///     Creates a flattened object based on the aggregate given.
	/// </summary>
	/// <param name="aggregate">Aggregate to copy properties from</param>
	public object? Flatten(object? aggregate)
	{
		if (aggregate is null)
		{
			return null;
		}

		var type = aggregate.GetType();
		if (!ShouldFlattenType(type))
		{
			return aggregate;
		}

		var flatResult = CreateFlattenedInstance(type);

		CopyValuesToFlatResult(aggregate, flatResult, flatResult.GetType());

		return flatResult!;
	}

	/// <summary>
	///     Retrieves the type of the flattened version of type T.
	/// </summary>
	/// <typeparam name="T">Type to flatten</typeparam>
	public Type GetFlattenedType<T>()
	{
		return _flatTypeMap.GetOrAdd(typeof(T), CreateFlattenedType);
	}

	/// <summary>
	///     Unflattens the given flattenedAggregate into its original type T.
	/// </summary>
	/// <typeparam name="T">The original type the flattenedObject is based on</typeparam>
	/// <param name="flattenedObject">A flattened object</param>
	public T Unflatten<T>(object flattenedObject)
	{
		var result = TypeInstantiator.CreateInstance<T>();
		var destinationProperties = GetProperties(typeof(T));
		var sourceProperties = GetProperties(flattenedObject.GetType());
		Dictionary<string, (object Value, IReadOnlyExtendedPropertyInfoCollection Properties)> paths = new();
		foreach (var sourceProperty in sourceProperties)
		{
			var sourceValue = sourceProperty.GetValue(flattenedObject);
			if (destinationProperties.TryGetValue(sourceProperty.Name, out var destinationProperty))
			{
				sourceValue = ConvertTypeToUnflattenIfNecessary(sourceValue, destinationProperty!.Type);
				destinationProperty.SetValue(result, sourceValue);
			}
			else // We're dealing with a value object which means a nested destination
			{
				var parts = sourceProperty.Name.Split('_');
				var path = string.Join("_", parts[..^1]);
				if (paths.TryGetValue(path, out var destination))
				{
					destinationProperty = destination.Properties[parts.Last()];
					sourceValue = ConvertTypeToUnflattenIfNecessary(sourceValue, destinationProperty.Type);
					destinationProperty.SetValue(destination.Value, sourceValue);
				}
				else
				{
					paths[path] = CopyValueToNestedDestination(result, parts, sourceValue);
				}
			}
		}

		return result;
	}

	private (object Value, IReadOnlyExtendedPropertyInfoCollection Properties) CopyValueToNestedDestination<T>(
		T result,
		string[] parts,
		object? sourceValue)
	{
		ArgumentNullException.ThrowIfNull(result);
		object destinationObject = result;
		var destinationProperties = GetProperties(destinationObject.GetType());
		ExtendedPropertyInfo destinationProperty;
		foreach (var part in parts
			         [..^1]) // Ensure properties exist for everything up til the one to set the source property on
		{
			destinationProperty = destinationProperties[part];
			var existingValue = destinationProperty.GetValue(destinationObject);
			if (existingValue is null)
			{
				existingValue = TypeInstantiator.CreateInstance(destinationProperty.Type);
				destinationProperty.SetValue(destinationObject, existingValue);
			}

			destinationObject = existingValue;
			destinationProperties = GetProperties(destinationObject.GetType());
		}

		destinationProperty = destinationProperties[parts.Last()];
		sourceValue = ConvertTypeToUnflattenIfNecessary(sourceValue, destinationProperty.Type);
		destinationProperty.SetValue(destinationObject, sourceValue);
		return (destinationObject, destinationProperties);
	}

	private object? ConvertTypeToUnflattenIfNecessary(object? sourceValue, Type destinationType)
	{
		if (sourceValue is null)
		{
			return sourceValue;
		}

		if (_typeConverters.TryGetValue(destinationType, out var typeConverter))
		{
			return typeConverter.ConvertToComplex(sourceValue);
		}
		if (destinationType.IsGenericType)
		{
			var singleGenericArgumentType = destinationType.GetGenericArguments().SingleOrDefault();
			if (singleGenericArgumentType is not null &&
			    _typeConverters.TryGetValue(singleGenericArgumentType, out typeConverter))
			{
				return typeConverter.ConvertToComplex(sourceValue!);
			}
		}

		return sourceValue;
	}

	public bool ShouldFlattenType<T>()
	{
		return ShouldFlattenType(typeof(T));
	}

	private bool ShouldFlattenType(Type type)
	{
		return _shouldFlattenTypeMap.GetOrAdd(type, t =>
		{
			if (_typeConverters.ContainsKey(type))
			{
				return true;
			}

			if (t.IsSimpleOrBuiltIn(_treatAsBuiltInType))
			{
				return false;
			}

			foreach (var prop in GetProperties(t))
			{
				if (!prop.Type.IsSimpleOrBuiltIn(_treatAsBuiltInType))
				{
					return true;
				}
			}

			return false;
		});
	}

	private void CopyValuesToFlatResult(object aggregate, object flatResult, Type flatType, string prefix = "")
	{
		var destinationProperties = GetProperties(flatType);

		foreach (var prop in GetProperties(aggregate.GetType()))
		{
			var propValue = prop.GetValue(aggregate);
			var destinationPropType = prop.Type;
			if (_typeConverters.TryGetValue(prop.Type, out var typeConverter) && propValue is not null)
			{
				propValue = typeConverter.ConvertToSimple(propValue);
				destinationPropType = typeConverter.SimpleType;
			}
			else if (prop.Type.IsGenericType &&
			         prop.Type.GetGenericArguments().SingleOrDefault(type => _typeConverters.ContainsKey(type)) !=
			         null)
			{
				if (prop.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					if (_typeConverters.TryGetValue(prop.Type.GetGenericArguments().Single(), out typeConverter))
					{
						if (propValue is not null)
						{
							propValue = typeConverter.ConvertToSimple(propValue);
						}
						destinationPropType = typeConverter.SimpleType;
					}
				}
				else
				{
					propValue = ConvertCollectionToSimple(prop, propValue, ref typeConverter);
				}
			}

			if (destinationPropType.IsSimpleOrBuiltIn(_treatAsBuiltInType))
			{
				destinationProperties[$"{prefix}{prop.Name}"].SetValue(flatResult, propValue);
			}
			else
			{
				if (propValue is not null)
				{
					CopyValuesToFlatResult(propValue, flatResult, flatType, $"{prefix}{prop.Name}_");
				}
			}
		}
	}

	private object? ConvertCollectionToSimple(ExtendedPropertyInfo prop,
		object? propValue,
		ref ITypeConverter? typeConverter)
	{
		if (propValue is not IEnumerable enumerable ||
		    !_typeConverters.TryGetValue(prop.Type.GetGenericArguments()[0], out typeConverter))
		{
			return propValue;
		}

		var enumerator = enumerable.GetEnumerator();
		var count = 0;
		while (enumerator.MoveNext())
		{
			count++;
		}

		enumerator.Reset();
		var destinationArray = Array.CreateInstance(typeConverter.SimpleType, count);
		var i = 0;
		while (enumerator.MoveNext())
		{
			if (enumerator.Current != null)
			{
				destinationArray.SetValue(typeConverter.ConvertToSimple(enumerator.Current), i++);
			}
		}

		return destinationArray;
	}

	private object CreateFlattenedInstance(Type type)
	{
		var flatType = _flatTypeMap.GetOrAdd(type, CreateFlattenedType);
		return TypeInstantiator.CreateInstance(flatType);
	}

	private Type CreateFlattenedType(Type type)
	{
		var tb = _moduleBuilder.DefineType($"{type.Name}Flattened{GenerateStrippedGuid()}", TypeAttributes.Public);
		CreateProperties(type, tb);

		return tb.CreateType()!;
	}

	private void CreateProperties(Type type, TypeBuilder typeBuilder, string prefix = "")
	{
		var properties = GetProperties(type);

		foreach (var prop in properties)
		{
			if (_typeConverters.TryGetValue(prop.Type, out var typeConverter))
			{
				CreateProperty(typeBuilder, prefix, prop.Name, typeConverter.SimpleType);
			}
			else if (prop.Type.IsGenericType
			         && (prop.Type.IsNullable() || prop.Type.IsGenericEnumerable())
			         && prop.Type.GetGenericArguments().SingleOrDefault(genericParamType =>
				         _typeConverters.ContainsKey(genericParamType)) is not null)
			{
				var convertedGenericParams = prop.Type
					.GetGenericArguments()
					.Select(genericArgumentType =>
						_typeConverters.TryGetValue(genericArgumentType, out var argumentTypeConverter)
							? argumentTypeConverter.SimpleType
							: genericArgumentType)
					.ToArray();

				var convertedGenericType = prop.Type.IsNullable()
					? convertedGenericParams.Single() // The TypeConverter has already wrapped the simple type in Nullable
					: prop.Type.GetGenericTypeDefinition().MakeGenericType(convertedGenericParams);
				CreateProperty(typeBuilder, prefix, prop.Name, convertedGenericType);
			}
			else if (prop.Type.IsSimpleOrBuiltIn(_treatAsBuiltInType))
			{
				CreateProperty(typeBuilder, prefix, prop);
			}
			else
			{
				CreateProperties(prop.Type, typeBuilder, $"{prefix}{prop.Name}_");
			}
		}
	}

	private static void CreateProperty(TypeBuilder typeBuilder, string prefix, ExtendedPropertyInfo prop)
	{
		CreateProperty(typeBuilder, prefix, prop.Name, prop.Type);
	}

	private static void CreateProperty(TypeBuilder typeBuilder,
		string prefix,
		string propertyName,
		Type propertyType)
	{
		var fieldBuilder =
			typeBuilder.DefineField($"_{prefix}{propertyName}", propertyType, FieldAttributes.Private);
		var propertyBuilder = typeBuilder.DefineProperty($"{prefix}{propertyName}", PropertyAttributes.HasDefault,
			propertyType, null);

		var propVisibility = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

		// Define the "get" accessor method for CustomerName.
		var getMethod = typeBuilder.DefineMethod($"get_{prefix}{propertyName}", propVisibility, propertyType,
			Type.EmptyTypes);

		var custNameGetIL = getMethod.GetILGenerator();

		custNameGetIL.Emit(OpCodes.Ldarg_0);
		custNameGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
		custNameGetIL.Emit(OpCodes.Ret);

		// Define the "set" accessor method for CustomerName.
		var setMethod = typeBuilder.DefineMethod($"set_{prefix}{propertyName}", propVisibility, null,
			new[] { propertyType });

		var custNameSetIL = setMethod.GetILGenerator();

		custNameSetIL.Emit(OpCodes.Ldarg_0);
		custNameSetIL.Emit(OpCodes.Ldarg_1);
		custNameSetIL.Emit(OpCodes.Stfld, fieldBuilder);
		custNameSetIL.Emit(OpCodes.Ret);

		// Last, we must map the two methods created above to our PropertyBuilder to
		// their corresponding behaviors, "get" and "set" respectively.
		propertyBuilder.SetGetMethod(getMethod);
		propertyBuilder.SetSetMethod(setMethod);
	}

	private static string GenerateStrippedGuid()
	{
		return Guid.NewGuid().ToString().Replace("-", string.Empty);
	}

	private static IReadOnlyExtendedPropertyInfoCollection GetProperties(Type type)
	{
		if (!_typeProperties.TryGetValue(type, out var result))
		{
			result = TypePropertiesCache.GetProperties(type);
		}

		return result;
	}
}