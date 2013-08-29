using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class type_instance_provider
	{
		[Fact]
		public void should_resolve_type_with_constructor_dependencies()
		{
			var container = new Container(x => x.For<IBar>().Use<Bar>());

			var foo = container.Get<Foo>();

			foo.Bar.Should().NotBeNull();
		}

		[Fact]
		public void should_reevaluate_selected_ctor_when_config_changes()
		{
			var container = new Container(x => x.For<Foo>().Use<Foo>());

			var foo = container.Get<Foo>();

			foo.Bar.Should().BeNull();

			container.Configure(x => x.For<IBar>().Use<Bar>());

			foo = container.Get<Foo>();

			foo.Bar.Should().NotBeNull();
		}

		private class Foo
		{
			public Foo() : this(null) { }
			public Foo(IBar bar)
			{
				Bar = bar;
			}

			public IBar Bar { get; private set; }
		}

		private interface IBar { }
		private class Bar : IBar { }

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.For(typeof(IFoobar<>)).Use(typeof(Foobar<>)));

			var foobar = container.Get<IFoobar<int>>();

			foobar.Should().BeOfType<Foobar<int>>();
		}

		private interface IFoobar<T> { }
		private class Foobar<T> : IFoobar<T> { }
	}
}