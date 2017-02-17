using System;
using System.Collections.Generic;

namespace Maestro.Tests
{
	internal static class Extensions
	{
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items) action(item);
		}
	}
}
