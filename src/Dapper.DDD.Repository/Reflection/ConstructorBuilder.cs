using System.Reflection;
using System.Reflection.Emit;

namespace Dapper.DDD.Repository.Reflection;

internal static class ConstructorBuilder
{
	public static ConstructorInfo CreateEmptyConstructor(Type type)
	{
		var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Dynamic"), AssemblyBuilderAccess.Run);
		var moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic");
		var typeBuilder = moduleBuilder.DefineType(type.Name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, type);

		var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, CallingConventions.Standard, Type.EmptyTypes);
		var ilGenerator = constructorBuilder.GetILGenerator();
		ilGenerator.Emit(OpCodes.Ret);

		var generatedType = typeBuilder.CreateType();
		var ctorInfo = generatedType!.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
		return ctorInfo!;
	}
}