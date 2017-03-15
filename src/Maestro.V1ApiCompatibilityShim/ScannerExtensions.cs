using System;
using Maestro.Configuration;
using Maestro.Conventions;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static Scanner Where(this Scanner scanner, Func<Type, bool> predicate)
		{
			return scanner.Matching(predicate);
		}

		public static void Using(this Scanner scanner, IConvention convention)
		{
			scanner.With(convention);
		}
	}
}