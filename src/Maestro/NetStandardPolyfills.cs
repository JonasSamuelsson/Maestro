using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
			var interfaces = new HashSet<Type>();

			if (type.IsInterface())
			{
				interfaces.Add(type);
			}

			foreach (var @interface in type.GetTypeInfo().ImplementedInterfaces)
			{
				interfaces.Add(@interface);
			}

			return interfaces;
		}

		public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
		{
			return type.GetTypeInfo().DeclaredConstructors.Where(x => !x.IsStatic);
		}

		public static PropertyInfo GetProperty(this Type type, string name)
		{
			return (from p in type.GetRuntimeProperties()
					  where p.Name == name
					  let method = p.GetMethod ?? p.SetMethod
					  let isStatic = method.IsStatic
					  where !isStatic
					  select p).Single();
		}

		public static MethodInfo GetMethod(this Type type, string name, Type[] parameterTypes)
		{
			var method = type.GetMethodOrNull(name, parameterTypes);
			if (method != null) return method;
			throw new InvalidOperationException($"Can't get method {type.FullName}.{name}({string.Join(", ", parameterTypes.Select(x => x.FullName))})");
		}

		public static MethodInfo GetMethodOrNull(this Type type, string name, Type[] parameterTypes)
		{
			foreach (var method in type.GetRuntimeMethods())
			{
				if (method.Name != name) continue;

				var @params = method.GetParameters();
				if (@params.Length != parameterTypes.Length) continue;

				for (var i = 0; i < @params.Length; i++)
				{
					if (@params[i].ParameterType != parameterTypes[i]) goto nextMethod;
				}

				return method;

				nextMethod:
				;
			}

			return null;
		}

		public static MethodInfo GetSetMethod(this PropertyInfo property)
		{
			return property.SetMethod;
		}

		public static Type[] GetGenericArguments(this Type type)
		{
			var typeInfo = type.GetTypeInfo();
			return typeInfo.ContainsGenericParameters
				? typeInfo.GenericTypeParameters
				: typeInfo.GenericTypeArguments;
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
