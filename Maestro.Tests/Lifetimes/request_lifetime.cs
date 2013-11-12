using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class request_lifetime
	{
		[Fact]
		public void the_same_instance_should_be_used_during_one_request()
		{
			var container = new Container(x => x.For<object>().Use<object>().Lifetime.RequestSingleton());

			var foo = container.Get<Foo>();

			foo.Object.Should().Be(foo.Bar.Object);
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