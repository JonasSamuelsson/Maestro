using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro
{
	internal static class TypeExtensions
	{
		private static IEnumerable<Type> GetClasses(this Type @class)
		{
			if (!@class.GetTypeInfo().IsClass)
				throw new InvalidOperationException();

			do
			{
				yield return @class;
				@class = @class.GetTypeInfo().BaseType;
			} while (@class != null);
		}

		public static bool IsConcreteClosedClass(this Type type)
		{
			return type.IsConcreteClass() && !type.GetTypeInfo().IsGenericTypeDefinition;
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition, out Type genericType)
		{
			genericType = null;

			if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
				throw new ArgumentException();

			if (!type.IsConcreteClosedClass())
				return false;

			var types = genericTypeDefinition.GetTypeInfo().IsClass ? type.GetClasses() : type.GetTypeInfo().ImplementedInterfaces;
			foreach (var prospect in types)
			{
				if (!prospect.GetTypeInfo().IsGenericType) continue;
				if (prospect.GetGenericTypeDefinition() != genericTypeDefinition) continue;
				genericType = prospect;
				return true;
			}

			return false;
		}

		public static bool IsConcreteClass(this Type type)
		{
			return (type.GetTypeInfo().IsClass && type != typeof(string) && !type.GetTypeInfo().IsAbstract);
		}

		public static bool IsConcreteSubClassOf(this Type type, Type basetype)
		{
			if (type == basetype)
				return false;

			if (type.GetTypeInfo().IsAbstract || !type.GetTypeInfo().IsClass)
				return false;

			if (type.GetTypeInfo().IsGenericTypeDefinition ^ basetype.GetTypeInfo().IsGenericTypeDefinition)
				return false;

			if (!basetype.GetTypeInfo().IsGenericTypeDefinition)
				return basetype.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

			var args = basetype.GetTypeInfo().GenericTypeParameters;
			var typeArgs = type.GetTypeInfo().GenericTypeParameters;

			if (args.Length != typeArgs.Length)
				return false;

			var typeDefinitions =
				from t in (basetype.GetTypeInfo().IsClass ? type.GetClasses() : type.GetTypeInfo().ImplementedInterfaces)
				where t.GetTypeInfo().IsGenericType
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				let tArgs = t.GetTypeInfo().GenericTypeParameters.Union(t.GetTypeInfo().GenericTypeArguments).Where(x => x.FullName == null).Distinct().ToArray()
				where args.Length == tArgs.Length
				select t.GetGenericTypeDefinition();

			return typeDefinitions.Any(x => x == basetype);
		}
	}
}