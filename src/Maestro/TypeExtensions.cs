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
			Type genericType;
			return type.IsConcreteClassClosing(genericTypeDefinition, out genericType);
		}

		public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition, out Type genericType)
		{
			genericType = null;

			if (!genericTypeDefinition.IsGenericTypeDefinition())
				throw new ArgumentException();

			if (!type.IsConcreteClosedClass())
				return false;

			var types = genericTypeDefinition.IsClass() ? type.GetClasses() : type.GetInterfaces();
			foreach (var prospect in types)
			{
				if (!prospect.IsGenericType()) continue;
				if (prospect.GetGenericTypeDefinition() != genericTypeDefinition) continue;
				genericType = prospect;
				return true;
			}

			return false;
		}

		public static bool IsConcreteClass(this Type type)
		{
			return type.IsClass() && type != typeof(string) && !type.IsAbstract();
		}

		public static bool IsConcreteSubClassOf(this Type type, Type basetype)
		{
			if (type == basetype)
				return false;

			if (type.IsAbstract() || !type.IsClass())
				return false;

			if (type.IsGenericTypeDefinition() ^ basetype.IsGenericTypeDefinition())
				return false;

			if (!basetype.IsGenericTypeDefinition())
				return basetype.IsAssignableFrom(type);

			var args = basetype.GetGenericArguments();
			var typeArgs = type.GetGenericArguments();

			if (args.Length != typeArgs.Length)
				return false;

			var typeDefinitions =
				from t in basetype.IsClass() ? type.GetClasses() : type.GetInterfaces()
				where t.IsGenericType()
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				let tArgs = t.GetGenericArguments().Where(x => x.FullName == null).Distinct().ToArray()
				where args.Length == tArgs.Length
				select t.GetGenericTypeDefinition();

			return typeDefinitions.Any(x => x == basetype);
		}
	}
}