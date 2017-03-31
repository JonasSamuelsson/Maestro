using System;
using Maestro.Configuration;
using Maestro.Conventions;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static IScanner Where(this IScanner scanner, Func<Type, bool> predicate)
		{
			return scanner.Where(predicate);
		}

		public static void Using(this IScanner scanner, IConvention convention)
		{
			scanner.With(convention);
		}
	}
}