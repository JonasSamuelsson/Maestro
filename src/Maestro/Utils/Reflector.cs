using System;
using System.Collections.Generic;

namespace Maestro.Utils
{
	internal static class Reflector
	{
		public static bool IsGenericEnumerable(Type type)
		{
			return IsGenericEnumerable(type, out _);
		}

		public static bool IsGenericEnumerable(Type type, out Type elementType)
		{
			elementType = null;
			if (!type.IsGenericType()) return false;
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition != typeof(IEnumerable<>)) return false;
			elementType = type.GetGenericArguments()[0];
			return true;
		}

		public static bool IsPrimitive(Type type)
		{
			return type.IsValueType() || type == typeof(string) || type == typeof(object);
		}

		public static bool IsGeneric(Type type, out Type genericTypeDefinition, out Type[] genericArguments)
		{
			genericTypeDefinition = null;
			genericArguments = null;

			if (!type.IsGenericType())
			{
				return false;
			}

			if (type.IsGenericTypeDefinition())
			{
				return false;
			}

			genericTypeDefinition = type.GetGenericTypeDefinition();
			genericArguments = type.GetGenericArguments();
			return true;
		}
	}
}