using System;
using System.Collections.Concurrent;

namespace Maestro.Internals
{
	internal class StrongRefTransientDisposableTracker : TransientDisposableTracker
	{
		private readonly ConcurrentBag<IDisposable> _disposables = new ConcurrentBag<IDisposable>();

		public override void Add(IDisposable disposable)
		{
			_disposables.Add(disposable);
		}

		public override void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				try
				{
					disposable?.Dispose();
				}
				catch
				{
					// ignored
				}
			}
		}
	}
}