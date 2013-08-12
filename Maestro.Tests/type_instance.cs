using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class type_instance
	{
		[Fact]
		public void selected_ctor_should_be_reevaluated_when_config_changes()
		{
			var name = "qwerty";
			var dependency = "dependency";

			var container = new Container(x =>
			{
				x.For<Foo>().Use<Foo>();
				x.For<object>().Use<object>();
			});
			var foo = container.Get<Foo>(name);

			foo.Object.Should().BeOfType<object>();

			container.Configure(x => x.Add<object>(name).Use(dependency));

			foo = container.Get<Foo>(name);

			foo.Object.Should().Be(dependency);
		}

		private class Foo
		{
			public Foo(object o)
			{
				Object = o;
			}

			public object Object { get; private set; }
		}
	}
}