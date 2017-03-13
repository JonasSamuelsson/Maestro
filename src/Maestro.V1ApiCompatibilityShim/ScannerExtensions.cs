using System;
using Maestro.Configuration;
using Maestro.Conventions;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static ScanExpression Where(this ScanExpression expression, Func<Type, bool> predicate)
		{
			return expression.Matching(predicate);
		}

		public static void Using(this ScanExpression expression, IConvention convention)
		{
			expression.With(convention);
		}
	}
}