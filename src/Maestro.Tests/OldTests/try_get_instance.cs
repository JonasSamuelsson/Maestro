using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class try_get_instance
	{
		[Fact]
		public void should_return_false_and_instance_should_be_null_if_provided_type_cant_be_resolved()
		{
			var container = new Container();

			object o;
			container.TryGetService(typeof(IDisposable), out o).ShouldBe(false);
			o.ShouldBe(null);

			IDisposable disposable;
			container.TryGetService(out disposable).ShouldBe(false);
			disposable.ShouldBe(null);

			container.Configure(x => x.Service<IDisposable>("xyz").Use.Type<Disposable>());

			container.TryGetService(out disposable).ShouldBe(false);
			disposable.ShouldBe(null);
		}

		[Todo]
		public void should_return_true_and_instance_should_not_be_null_if_provided_type_can_be_resolved()
		{
			var container = new Container();

			object o;
			container.TryGetService(typeof(object), out o).ShouldBe(true);
			o.ShouldNotBe(null);

			container.TryGetService(out o).ShouldBe(true);
			o.ShouldNotBe(null);

			container.Configure(x => x.Service<IDisposable>().Use.Type<Disposable>());

			container.TryGetService(typeof(IDisposable), out o).ShouldBe(true);
			o.ShouldNotBe(null);

			IDisposable disposable;
			container.TryGetService(out disposable).ShouldBe(true);
			disposable.ShouldNotBe(null);

			var namedDisposable = new Disposable();
			container.Configure(x => x.Service<IDisposable>("xyz").Use.Instance(namedDisposable));

			TestClassWithDisposable testClassWithDisposable;
			container.TryGetService(out testClassWithDisposable).ShouldBe(true);
			testClassWithDisposable.ShouldNotBe(null);
			testClassWithDisposable.Disposable.ShouldNotBe(namedDisposable);

			container.TryGetService("xyz", out testClassWithDisposable).ShouldBe(true);
			testClassWithDisposable.ShouldNotBe(null);
			testClassWithDisposable.Disposable.ShouldBe(namedDisposable);
		}

		private class TestClassWithDisposable
		{
			public TestClassWithDisposable(IDisposable disposable)
			{
				Disposable = disposable;
			}

			public IDisposable Disposable { get; private set; }
		}

		private class Disposable : IDisposable
		{
			public void Dispose() { }
		}
	}
}