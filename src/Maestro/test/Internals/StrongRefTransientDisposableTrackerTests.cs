using Maestro.Internals;
using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class StrongRefTransientDisposableTrackerTests
	{
		[Fact]
		public void ShouldDisposeItems()
		{
			var tracker = new StrongRefTransientDisposableTracker();

			var disposable = new Disposable();

			tracker.Add(disposable);

			tracker.Dispose();

			disposable.Disposed.ShouldBeTrue();
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