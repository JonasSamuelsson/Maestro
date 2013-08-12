using System;

namespace Maestro
{
	internal static class TypeHelper
	{
		public static void AssertTypeIsSupported(Type type)
		{
			if (!IsTypeSupported(type))
				throw new NotSupportedException(string.Format("Type '{0}' is not supported.", type.FullName));
		}

		public static bool IsTypeSupported(Type type)
		{
			return !type.IsValueType && type != typeof(string);
		}

		public static bool IsConcreteClosedClass(Type type)
		{
			return type.IsClass && type != typeof(string) && !type.IsAbstract && !type.IsGenericTypeDefinition;
		}
	}
}