using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_dependencies
	{
		[Fact]
		public void get_default_should_use_default_instance_for_dependencies()
		{
			var name = "1";
			var container = new Container(x =>
			{
				x.For<IRoot>().Use<Root>();
				x.For<IFoo>().Use<Foo>();
				x.For<IBar>().Use<Bar>();
				x.Add<IRoot>(name).Use<Root1>();
				x.Add<IBar>(name).Use<Bar1>();
			});

			var root = container.Get<IRoot>();

			root.Should().BeOfType<Root>();
			root.Foo.Should().BeOfType<Foo>();
			root.Foo.Bar.Should().BeOfType<Bar>();
		}

		[Fact]
		public void get_named_should_use_instance_with_same_name_or_fallback_to_default_instance_for_dependencies()
		{
			var name = "1";
			var container = new Container(x =>
			{
				x.For<IRoot>().Use<Root>();
				x.For<IFoo>().Use<Foo>();
				x.For<IBar>().Use<Bar>();
				x.Add<IRoot>(name).Use<Root1>();
				x.Add<IBar>(name).Use<Bar1>();
			});

			var root = container.Get<IRoot>(name);

			root.Should().BeOfType<Root1>();
			root.Foo.Should().BeOfType<Foo>();
			root.Foo.Bar.Should().BeOfType<Bar1>();
		}

		[Fact]
		public void get_all_should_use_dependencies_with_same_name_as_top_instance_or_default()
		{
			var name = "1";
			var container = new Container(x =>
			{
				x.For<IRoot>().Use<Root>();
				x.For<IFoo>().Use<Foo>();
				x.For<IBar>().Use<Bar>();
				x.Add<IRoot>(name).Use<Root1>();
				x.Add<IBar>(name).Use<Bar1>();
			});

			var defaultRoot = container.Get<IRoot>();
			var namedRoot = container.Get<IRoot>(name);

			defaultRoot.Should().BeOfType<Root>();
			defaultRoot.Foo.Should().BeOfType<Foo>();
			defaultRoot.Foo.Bar.Should().BeOfType<Bar>();
			namedRoot.Should().BeOfType<Root1>();
			namedRoot.Foo.Should().BeOfType<Foo>();
			namedRoot.Foo.Bar.Should().BeOfType<Bar1>();
		}

		private interface IRoot { IFoo Foo { get; } }
		private interface IFoo { IBar Bar { get; } }
		private interface IBar { }

		private class Root : IRoot
		{
			public Root(IFoo foo) { Foo = foo; }
			public IFoo Foo { get; private set; }
		}
		private class Root1 : IRoot
		{
			public Root1(IFoo foo) { Foo = foo; }
			public IFoo Foo { get; private set; }
		}

		private class Foo : IFoo
		{
			public Foo(IBar bar) { Bar = bar; }
			public IBar Bar { get; private set; }
		}

		private class Bar : IBar { }
		private class Bar1 : IBar { }
	}
}