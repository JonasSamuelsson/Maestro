using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class StrongRefTransientDisposableTracker : TransientDisposableTracker
	{
		private readonly List<IDisposable> _disposables = new List<IDisposable>();

		public override void Add(IDisposable disposable)
		{
			_disposables.Add(disposable);
		}

		public override void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				disposable.Dispose();
			}
		}
	}
}