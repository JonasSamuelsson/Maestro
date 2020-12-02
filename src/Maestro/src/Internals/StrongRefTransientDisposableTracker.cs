using System;

namespace Maestro.Internals
{
	internal class StrongRefTransientDisposableTracker : TransientDisposableTracker<IDisposable>
	{
		protected override IDisposable CreateItem(IDisposable disposable)
		{
			return disposable;
		}

		protected override void DisposeItem(IDisposable item)
		{
			item?.Dispose();
		}
	}
}