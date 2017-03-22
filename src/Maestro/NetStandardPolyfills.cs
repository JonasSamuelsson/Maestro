using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Maestro
{
	internal static class NetStandardPolyfills
	{
		public static IEnumerable<Type> GetTypes(this Assembly assembly)
		{
			return assembly.DefinedTypes.Select(x => x.AsType());
		}

		public static bool IsInterface(this Type type)
		{
			return type.GetTypeInfo().IsInterface;
		}

		public static Assembly GetAssembly(this Type type)
		{
			return type.GetTypeInfo().Assembly;
		}

		public static Type GetBaseType(this Type type)
		{
			return type.GetTypeInfo().BaseType;
		}

		public static IEnumerable<Type> GetInterfaces(this Type type)
		{
			return type.GetTypeInfo().ImplementedInterfaces;
		}

		public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
		{
			return type.GetTypeInfo().DeclaredConstructors;
		}

		public static PropertyInfo GetProperty(this Type type, string name)
		{
			return type.GetRuntimeProperty(name);
		}

		public static MethodInfo GetMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetRuntimeMethod(name, parameters);
		}

		public static MethodInfo GetSetMethod(this PropertyInfo property)
		{
			return property.SetMethod;
		}

		public static Type[] GetGenericArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}

		public static bool IsAbstract(this Type type)
		{
			return type.GetTypeInfo().IsAbstract;
		}

		public static bool IsAssignableFrom(this Type baseType, Type derivedType)
		{
			return baseType.GetTypeInfo().IsAssignableFrom(derivedType.GetTypeInfo());
		}

		public static bool IsClass(this Type type)
		{
			return type.GetTypeInfo().IsClass;
		}

		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.GetTypeInfo().IsGenericTypeDefinition;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.GetTypeInfo().IsGenericType;
		}

		public static bool IsValueType(this Type type)
		{
			return type.GetTypeInfo().IsValueType;
		}
	}
}
