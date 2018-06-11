using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class singleton_lifetime
	{
		[Fact]
		public void singleton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.Use<object>().Type<object>().Singleton());

			var o1 = container.GetService<object>();
			var o2 = container.GetService<object>();
			var o3 = container.GetScopedContainer().GetService<object>();

			o1.ShouldBe(o2);
			o1.ShouldBe(o3);
		}

		[Fact]
		public void should_dispose_instances_with_root_container()
		{
			var rootContainer = new Container(x => x.Use<Disposable>().Self().Singleton());
			var scopedContainer = rootContainer.GetScopedContainer();

			var o1 = rootContainer.GetService<Disposable>();
			var o2 = scopedContainer.GetService<Disposable>();

			scopedContainer.Dispose();

			o1.Disposed.ShouldBeFalse();
			o2.Disposed.ShouldBeFalse();

			rootContainer.Dispose();

			o1.Disposed.ShouldBeTrue();
			o2.Disposed.ShouldBeTrue();
		}

		private class Disposable : IDisposable
		{
			public bool Disposed { get; private set; }

			public void Dispose()
			{
				Disposed = true;
			}
		}
	}
}