using System;
using System.Collections.Generic;

namespace Maestro
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var item in enumerable)
			{
				action(item);
				yield return item;
			}
		}
	}
}