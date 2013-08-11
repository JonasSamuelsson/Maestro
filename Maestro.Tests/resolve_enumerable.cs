using FluentAssertions;
using System.Linq;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_enumerable
	{
		[Fact]
		public void should_get_empty_enumerable_if_type_is_not_registered()
		{
			var enumerable = new Container().GetAll<object>();

			enumerable.Should().BeEmpty();
		}

		[Fact]
		public void should_try_to_get_dependencies_with_same_name_as_top_instance()
		{
			var @defaultDependency = "default";
			var namedDependency = "named";

			var name = "yada yada";
			var container = new Container(x =>
			{
				x.Add<Foo>().As<Foo>();
				x.Add<Foo>(name).As<Foo>();

				x.Default<object>().Is(@defaultDependency);
				x.Add<object>(name).As(namedDependency);
			});

			var foos = container.GetAll<Foo>().ToList();

			foos.Should().Contain(x => x.Object == defaultDependency);
			foos.Should().Contain(x => x.Object == namedDependency);
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