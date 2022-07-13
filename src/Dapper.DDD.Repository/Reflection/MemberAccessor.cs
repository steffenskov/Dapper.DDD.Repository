using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.DDD.Repository.Reflection;

internal class MemberAccessor
{
	#region Static

	private static Func<object, object?>? GetGetMethod(PropertyInfo property)
	{
		var method = property.GetGetMethod(true);
		if (method is null)
		{
			return null;
		}
		else
		{
			if (method.IsStatic)
			{
				throw new ArgumentException("Static properties not supported");
			}

			var dm = new DynamicMethod("__get_" + property.Name, typeof(object), new Type[] { typeof(object) }, property.DeclaringType!);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, property.DeclaringType!);
			il.Emit(OpCodes.Callvirt, method);
			if (property.PropertyType.IsValueType)
			{
				il.Emit(OpCodes.Box, property.PropertyType);
			}
			il.Emit(OpCodes.Ret);
			return (Func<object, object?>)dm.CreateDelegate(typeof(Func<object, object?>));
		}
	}

	private static Action<object, object?>? GetSetMethod(PropertyInfo property)
	{
		var method = property.GetSetMethod(true);
		if (method is null)
		{
			return null;
		}
		else
		{
			if (method.IsStatic)
			{
				throw new ArgumentException("Static properties not supported");
			}

			var dm = new DynamicMethod("__set_" + property.Name, null, new Type[] { typeof(object), typeof(object) }, property.DeclaringType!);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, property.DeclaringType!);
			il.Emit(OpCodes.Ldarg_1);
			if (property.PropertyType.IsValueType)
			{
				il.Emit(OpCodes.Unbox_Any, property.PropertyType);
			}
			else
			{
				il.Emit(OpCodes.Castclass, property.PropertyType);
			}
			il.Emit(OpCodes.Callvirt, method);
			il.Emit(OpCodes.Ret);
			return (Action<object, object?>)dm.CreateDelegate(typeof(Action<object, object?>));
		}
	}
	#endregion

	private readonly Func<object, object?>? _getter;
	private readonly Action<object, object?>? _setter;

	public bool HasGetter => _getter != null;
	public bool HasSetter => _setter != null;
	public string Name { get; }

	public MemberAccessor(PropertyInfo property)
	{
		if (property is null)
		{
			throw new ArgumentNullException(nameof(property));
		}

		if (property.DeclaringType is null)
		{
			throw new ArgumentException($"property.DeclaringType is null", nameof(property));
		}

		Name = property.Name;
		_getter = GetGetMethod(property);
		_setter = GetSetMethod(property);
	}

	public object? GetValue(object target)
	{
		if (_getter is null)
			throw new InvalidOperationException($"No getter for property: {Name}");
		return _getter(target);
	}

	public void SetValue(object target, object? value)
	{
		if (_setter is null)
			throw new InvalidOperationException($"No setter for property: {Name}");
		_setter(target, value);
	}
}
