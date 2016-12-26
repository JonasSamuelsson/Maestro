using System;
using Maestro.Configuration;

namespace Maestro
{
	public static class ScannerExtensions
	{
		public static IConventionExpression Where(this IConventionExpression expression, Func<Type, bool> predicate)
		{
			return expression.Matching(predicate);
		}
	}
}