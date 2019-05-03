using System;

namespace Maestro.Internals
{
	internal class DependencyDepthChecker
	{
		private int _current;
		private readonly int _max;

		internal DependencyDepthChecker(int maxDepth)
		{
			_max = maxDepth;
		}

		internal void Push()
		{
			if (_current++ < _max)
				return;

			throw new InvalidOperationException($"Exceeded max dependency depth {_max}.");
		}

		internal void Pop()
		{
			_current--;
		}
	}
}