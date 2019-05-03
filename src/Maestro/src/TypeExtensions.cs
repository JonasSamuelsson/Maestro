using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal static class TypeExtensions
	{
		private static IReadOnlyList<Type> GetClasses(this Type @class)
		{
			if (!@class.IsClass)
				throw new InvalidOperationException();

			var result = new List<Type>();

			do
			{
				result.Add(@class);
				@class = @class.BaseType;
			} while (@class != null);

			return result;
		}

		public static bool IsConcreteClosedClass(this Type type)
		{
			return type.IsConcreteClass() && !type.IsGenericTypeDefinition;
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition)
		{
			return type.IsConcreteClassClosing(genericTypeDefinition, out var _);
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition, out IReadOnlyCollection<Type> genericTypes)
		{
			genericTypes = null;

			if (!genericTypeDefinition.IsGenericTypeDefinition)
				throw new ArgumentException();

			if (!type.IsConcreteClosedClass())
				return false;

			return genericTypeDefinition.IsClass
				? IsConcreteClassClosingClass(type, genericTypeDefinition, ref genericTypes)
				: genericTypeDefinition.IsInterface
					? IsConcreteClassClosingInterface(type, genericTypeDefinition, ref genericTypes)
					: throw new InvalidOperationException();
		}

		private static bool IsConcreteClassClosingClass(Type type, Type genericClassDefinition, ref IReadOnlyCollection<Type> genericTypes)
		{
			while (type != null)
			{
				var t = type;
				type = type.BaseType;

				if (!t.IsGenericType)
					continue;

				var genericTypeDefinition = t.GetGenericTypeDefinition();

				if (genericTypeDefinition != genericClassDefinition)
					continue;

				genericTypes = new[] { t };
				return true;
			}

			return false;
		}

		private static bool IsConcreteClassClosingInterface(Type type, Type genericInterfaceDefinition, ref IReadOnlyCollection<Type> genericTypes)
		{
			var result = new List<Type>();

			var types = type.GetInterfaces();
			for (var i = 0; i < types.Length; i++)
			{
				var t = types[i];
				if (!t.IsGenericType) continue;
				if (t.GetGenericTypeDefinition() != genericInterfaceDefinition) continue;
				result.Add(t);
			}

			if (result.Count == 0)
				return false;

			genericTypes = result;
			return true;
		}

		public static bool IsConcreteClass(this Type type)
		{
			return type.IsClass && type != typeof(string) && !type.IsAbstract;
		}

		public static bool IsConcreteClassOf(this Type type, Type basetype, out Type genericType)
		{
			genericType = null;

			if (!type.IsConcreteClass())
				return false;

			if (!basetype.IsGenericTypeDefinition)
				return !type.IsGenericTypeDefinition && basetype.IsAssignableFrom(type);

			var typeIsGenericTypeDefinition = type.IsGenericTypeDefinition;
			var types = basetype.IsClass ? type.GetClasses() : type.GetInterfaces();

			for (var i = 0; i < types.Count; i++)
			{
				var t = types[i];
				if (!t.IsGenericType) continue;
				if (basetype != t.GetGenericTypeDefinition()) continue;
				if (!typeIsGenericTypeDefinition) genericType = t;
				return true;
			}

			return false;
		}

		internal static string ToFormattedString(this Type type)
		{
			var result = type?.FullName ?? type?.Name;

			if (string.IsNullOrEmpty(result))
				return string.Empty;

			if (type.IsGenericType)
			{
				var index = result.IndexOf('`');
				result = result.Substring(0, index);
				var typeArgs = type.GetGenericArguments().Select(ToFormattedString);
				result += $"<{string.Join(", ", typeArgs)}>";
			}

			return result;
		}
	}
}