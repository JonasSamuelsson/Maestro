using System.Linq;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_enumerable
	{
		[Fact]
		public void should_get_empty_enumerable_if_type_is_not_registered()
		{
			var container = new Container();

			var ints = container.GetAll<int>();
			var strings = container.GetAll<string>();

			ints.Should().BeEmpty();
			strings.Should().BeEmpty();
		}

		[Fact]
		public void should_get_enumerable_containing_registered_instances()
		{
			var @int = 8;
			var @string = "foobar";
			var container = new Container(x =>
													{
														x.For<int>().Use(@int);
														x.For<string>().Use(@string);
													});

			var ints = container.GetAll<int>();
			var strings = container.GetAll<string>();

			ints.Should().BeEquivalentTo(new[] { @int });
			strings.Should().BeEquivalentTo(new[] { @string });
		}

		[Fact(Skip = "todo")]
		public void should_try_to_get_dependencies_with_same_name_as_top_instance()
		{
			var @defaultDependency = "default";
			var namedDependency = "named";

			var name = "yada yada";
			var container = new Container(x =>
			{
				x.For<Foo>().Use<Foo>();
				x.For<Foo>(name).Use<Foo>();

				x.For<object>().Use(@defaultDependency);
				x.For<object>(name).Use(namedDependency);
			});

			var foos = container.GetAll<Foo>().ToList();

			foos.Should().Contain(x => (string)x.Object == defaultDependency);
			foos.Should().Contain(x => (string)x.Object == namedDependency);
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