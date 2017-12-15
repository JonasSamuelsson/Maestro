using System;
using System.Collections.Generic;

namespace Maestro
{
	internal static class TypeExtensions
	{
		private static IEnumerable<Type> GetClasses(this Type @class)
		{
			if (!@class.IsClass())
				throw new InvalidOperationException();

			do
			{
				yield return @class;
				@class = @class.GetBaseType();
			} while (@class != null);
		}

		public static bool IsConcreteClosedClass(this Type type)
		{
			return type.IsConcreteClass() && !type.IsGenericTypeDefinition();
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition)
		{
			return type.IsConcreteClassClosing(genericTypeDefinition, out var _);
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition, out IReadOnlyCollection<Type> genericTypes)
		{
			genericTypes = null;

			if (!genericTypeDefinition.IsGenericTypeDefinition())
				throw new ArgumentException();

			if (!type.IsConcreteClosedClass())
				return false;

			var result = new List<Type>();

			var types = genericTypeDefinition.IsClass() ? type.GetClasses() : type.GetInterfaces();
			foreach (var prospect in types)
			{
				if (!prospect.IsGenericType()) continue;
				if (prospect.GetGenericTypeDefinition() != genericTypeDefinition) continue;
				result.Add(prospect);
			}

			if (result.Count == 0)
				return false;

			genericTypes = result;
			return true;
		}

		public static bool IsConcreteClass(this Type type)
		{
			return type.IsClass() && type != typeof(string) && !type.IsAbstract();
		}

		public static bool IsConcreteClassOf(this Type type, Type basetype, out Type genericType)
		{
			genericType = null;

			if (type.IsAbstract() || !type.IsClass())
				return false;

			if (!basetype.IsGenericTypeDefinition())
				return !type.IsGenericTypeDefinition() && basetype.IsAssignableFrom(type);

			var typeIsGenericTypeDefinition = type.IsGenericTypeDefinition();
			var types = basetype.IsClass() ? type.GetClasses() : type.GetInterfaces();
			foreach (var t in types)
			{
				if (!t.IsGenericType()) continue;
				if (basetype != t.GetGenericTypeDefinition()) continue;
				if (!typeIsGenericTypeDefinition) genericType = t;
				return true;
			}

			return false;
		}
	}
}