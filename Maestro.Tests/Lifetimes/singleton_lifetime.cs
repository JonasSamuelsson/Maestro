using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class singleton_lifetime
	{
		[Fact]
		public void singleton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.For<object>().Use<object>().AsSingleton());

			var o1 = container.Get<object>();
			var o2 = container.Get<object>();

			o1.Should().Be(o2);

			var parent = new Container(x => x.For(typeof(IFoo<>)).Use(typeof(Foo<>)).AsSingleton());
			var child1 = parent.GetChildContainer();
			var child2 = parent.GetChildContainer();

			var foo1 = parent.Get<IFoo<int>>();
			var foo2 = child1.Get<IFoo<int>>();
			var foo3 = child2.Get<IFoo<int>>();

			foo1.Should().Be(foo2);
			foo2.Should().Be(foo3);
		}

		private interface IFoo<T> { }
		private class Foo<T> : IFoo<T> { }
	}
}