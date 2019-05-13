using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class WeakRefTransientDisposableTracker : TransientDisposableTracker
	{
		private readonly List<WeakReference<IDisposable>> _disposables = new List<WeakReference<IDisposable>>();

		public override void Add(IDisposable disposable)
		{
			_disposables.Add(new WeakReference<IDisposable>(disposable));
		}

		public override void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				if (!disposable.TryGetTarget(out var target))
					continue;
				target.Dispose();
			}
		}
	}
}