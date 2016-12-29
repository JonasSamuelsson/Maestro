using System;
using Maestro.Configuration;
using Maestro.Conventions;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static IConventionExpression Where(this IConventionExpression expression, Func<Type, bool> predicate)
		{
			return expression.Matching(predicate);
		}

		public static void Using(this IConventionExpression expression, IConvention convention)
		{
			expression.With(convention);
		}
	}
}