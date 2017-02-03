using System;

namespace Maestro.Conventions
{
	internal class LambdaFilter : IFilter
	{
		private readonly Func<Type, bool> _predicate;

		public LambdaFilter(Func<Type, bool> predicate)
		{
			_predicate = predicate;
		}

		public bool IsMatch(Type type)
		{
			return _predicate(type);
		}
	}
}