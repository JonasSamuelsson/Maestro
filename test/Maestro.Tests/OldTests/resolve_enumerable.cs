using Shouldly;
using System.Linq;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class resolve_enumerable
	{
		[Fact]
		public void should_get_empty_enumerable_if_type_is_not_registered()
		{
			var container = new Container();

			var ints = container.GetServices<int>();
			var strings = container.GetServices<string>();

			ints.ShouldBeEmpty();
			strings.ShouldBeEmpty();
		}

		[Fact]
		public void should_get_enumerable_containing_registered_instances()
		{
			var @int = 8;
			var @string = "foobar";
			var container = new Container(x =>
													{
														x.Add<int>().Instance(@int);
														x.Add<string>().Instance(@string);
													});

			var ints = container.GetServices<int>();
			var strings = container.GetServices<string>();

			ints.ShouldBe(new[] { @int });
			strings.ShouldBe(new[] { @string });
		}

		[Todo]
		public void should_try_to_get_dependencies_with_same_name_as_top_instance()
		{
			var @defaultDependency = "default";
			var namedDependency = "named";

			var name = "yada yada";
			var container = new Container(x =>
			{
				x.Add<Foo>().Type<Foo>();
				x.Add<Foo>().Named(name).Type<Foo>();

				x.Add<object>().Instance(defaultDependency);
				x.Add<object>().Named(name).Instance(namedDependency);
			});

			var foos = container.GetServices<Foo>().ToList();

			foos.ShouldContain(x => (string)x.Object == defaultDependency);
			foos.ShouldContain(x => (string)x.Object == namedDependency);
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