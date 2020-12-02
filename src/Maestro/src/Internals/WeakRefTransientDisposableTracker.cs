using System;

namespace Maestro.Internals
{
	internal class WeakRefTransientDisposableTracker : TransientDisposableTracker<WeakReference<IDisposable>>
	{
		protected override WeakReference<IDisposable> CreateItem(IDisposable disposable)
		{
			return new WeakReference<IDisposable>(disposable);
		}

		protected override void DisposeItem(WeakReference<IDisposable> item)
		{
			if (!item.TryGetTarget(out var disposable))
				return;

			disposable.Dispose();
		}
	}
}