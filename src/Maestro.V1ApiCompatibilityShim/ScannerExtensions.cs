using System;
using Maestro.Configuration;
using Maestro.Conventions;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static IScanExpression Where(this IScanExpression expression, Func<Type, bool> predicate)
		{
			return expression.Matching(predicate);
		}

		public static void Using(this IScanExpression expression, IConvention convention)
		{
			expression.With(convention);
		}
	}
}