using System;
using System.Collections.Generic;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal static class EnumerableTypeBuilder
	{
		private static readonly ThreadSafeDictionary<Type, Type> Cache = new ThreadSafeDictionary<Type, Type>();

		public static Type Get(Type elementType)
		{
			return Cache.GetOrAdd(elementType, x => typeof(IEnumerable<>).MakeGenericType(x));
		}
	}
}