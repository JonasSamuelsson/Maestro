using System;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class WeakRefTransientDisposableTrackerTests
	{
		[Fact]
		public void ShouldDisposeItems()
		{
			var tracker = new WeakRefTransientDisposableTracker();

			var disposable = new Disposable();

			tracker.Add(disposable);

			tracker.Dispose();

			disposable.Disposed.ShouldBeTrue();
		}

		[Fact]
		public void ShouldHandleGarbageCollectedDisposables()
		{
			var tracker = new WeakRefTransientDisposableTracker();

			var weakReference = AddDisposable(tracker);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			weakReference.TryGetTarget(out _).ShouldBeFalse();

			tracker.Dispose();
		}

		private static WeakReference<IDisposable> AddDisposable(WeakRefTransientDisposableTracker tracker)
		{
			var disposable = new Disposable();

			tracker.Add(disposable);

			return new WeakReference<IDisposable>(disposable);
		}

		public class Disposable : IDisposable
		{
			public bool Disposed { get; set; }

			public void Dispose()
			{
				Disposed = true;
			}
		}
	}
}