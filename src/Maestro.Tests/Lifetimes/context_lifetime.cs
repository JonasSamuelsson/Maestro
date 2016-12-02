using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class context_lifetime
	{
		[Fact]
		public void the_same_instance_should_be_returned_per_call_to_get()
		{
			var container = new Container(x => x.Service<object>().Use.Type<object>().Lifetime.Context());

			var foo1 = container.GetService<Foo>();
			var foo2 = container.GetService<Foo>();

			foo1.Object.Should().Be(foo1.Bar.Object);
			foo2.Object.Should().Be(foo2.Bar.Object);
			foo1.Object.Should().NotBe(foo2.Object);
		}

		private class Foo
		{
			public Foo(Bar bar, object o)
			{
				Bar = bar;
				Object = o;
			}

			public Bar Bar { get; private set; }
			public object Object { get; private set; }
		}

		private class Bar
		{
			public Bar(object o)
			{
				Object = o;
			}

			public object Object { get; private set; }
		}
	}
}