using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class container_scoped_lifetime
	{
		[Fact]
		public void container_scoped_instances_should_return_same_instance_per_container()
		{
			var container = new Container(x => x.Use<object>().Type<object>().Lifetime.ContainerScoped());
			var childContainer1 = container.GetChildContainer();
			var childContainer2 = container.GetChildContainer();

			var co1 = container.GetService<object>();
			var cc1o1 = childContainer1.GetService<object>();
			var cc2o1 = childContainer2.GetService<object>();
			var co2 = container.GetService<object>();
			var cc1o2 = childContainer1.GetService<object>();
			var cc2o2 = childContainer2.GetService<object>();

			co1.ShouldBe(co2);
			co1.ShouldNotBe(cc1o1);
			cc1o1.ShouldBe(cc1o2);
			cc1o1.ShouldNotBe(cc2o1);
			cc2o1.ShouldBe(cc2o2);
		}

		[Fact]
		public void should_handle_open_generics()
		{
			var container = new Container(x => x.Use(typeof(List<>)).Self().Lifetime.ContainerScoped());

			var ints1 = container.GetService<List<int>>();
			var ints2 = container.GetService<List<int>>();
			ints1.ShouldBe(ints2);

			var strings1 = container.GetService<List<string>>();
			var strings2 = container.GetService<List<string>>();
			strings1.ShouldBe(strings2);
		}

		[Fact]
		public void should_dispose_instances_with_container()
		{
			var rootContainer = new Container(x => x.Use<Disposable>().Self().Lifetime.ContainerScoped());
			var childContainer = rootContainer.GetChildContainer();

			var rootObject = rootContainer.GetService<Disposable>();
			var childObject = childContainer.GetService<Disposable>();

			childContainer.Dispose();

			rootObject.Disposed.ShouldBeFalse();
			childObject.Disposed.ShouldBeTrue();

			rootContainer.Dispose();

			rootObject.Disposed.ShouldBeTrue();
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