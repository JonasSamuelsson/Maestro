using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_dependencies
	{
		[Fact]
		public void should_get_instances_with_same_name_as_requested_instance_and_fall_back_to_default_instance()
		{
			var @default = "default";
			var name1 = "name1";
			var name2 = "name2";
			var defaultGrandChild = new GrandChild();
			var namedGrandChild = new GrandChild();
			var container = new Container(x =>
			{
				x.For<ParentWithSingleChild>().Use<ParentWithSingleChild>().OnCreate.SetProperty(y => y.Name, @default);
				x.Add<ParentWithSingleChild>(name1).Use<ParentWithSingleChild>().OnCreate.SetProperty(y => y.Name, name1);
				x.For<Child>().Use<Child>().OnCreate.SetProperty(y => y.Name, @default);
				x.Add<Child>(name2).Use<Child>().OnCreate.SetProperty(y => y.Name, name2);
				x.For<GrandChild>().Use(defaultGrandChild);
				x.Add<GrandChild>(name1).Use(namedGrandChild);
			});

			var defaultParent = container.Get<ParentWithSingleChild>();
			defaultParent.Name.Should().Be(@default);
			defaultParent.Child.Name.Should().Be(@default);
			defaultParent.Child.GrandChild.Should().Be(defaultGrandChild);

			var parent1 = container.Get<ParentWithSingleChild>(name1);
			parent1.Name.Should().Be(name1);
			parent1.Child.Name.Should().Be(@default);
			parent1.Child.GrandChild.Should().Be(namedGrandChild);

			var parent2 = container.Get<ParentWithSingleChild>(name2);
			parent2.Name.Should().Be(@default);
			parent2.Child.Name.Should().Be(name2);
			parent2.Child.GrandChild.Should().Be(@defaultGrandChild);

			//container.Get<ParentWithMultipleChildren>().Children.Should().OnlyContain(x => x.GrandChild == defaultGrandChild);
			//container.Get<ParentWithMultipleChildren>(name1).Children.Should().OnlyContain(x => x.GrandChild == namedGrandChild);
			//container.Get<ParentWithMultipleChildren>(name2).Children.Should().OnlyContain(x => x.GrandChild == defaultGrandChild);
		}

		[Fact]
		public void should_get_dependencies_with_same_name_as_top_instance_and_fall_back_to_default_instance()
		{
			var name1 = "abc";
			var name2 = "xyz";
			var defaultGrandChild = new GrandChild();
			var namedGrandChild = new GrandChild();
			var container = new Container(x =>
			{
				x.For<ParentWithSingleChild>().Use<ParentWithSingleChild>();
				x.Add<ParentWithSingleChild>(name1).Use<ParentWithSingleChild>();
				x.Add<ParentWithSingleChild>(name2).Use<ParentWithSingleChild>();
				x.For<GrandChild>().Use(defaultGrandChild);
				x.Add<GrandChild>(name1).Use(namedGrandChild);
			});

			container.GetAll<ParentWithSingleChild>().Select(x => x.Child.GrandChild).Should().OnlyContain(x => x == defaultGrandChild);

			container.Get<ParentWithMultipleChildren>().Children.Should().Contain(x => x.GrandChild == defaultGrandChild);
			container.Get<ParentWithMultipleChildren>(name1).Children.Should().Contain(x => x.GrandChild == namedGrandChild);
			container.Get<ParentWithMultipleChildren>(name2).Children.Should().Contain(x => x.GrandChild == defaultGrandChild);
		}

		private class ParentWithSingleChild
		{
			public ParentWithSingleChild(Child child)
			{
				Child = child;
			}

			public Child Child { get; private set; }
			public string Name { get; set; }
		}

		private class ParentWithMultipleChildren
		{
			public ParentWithMultipleChildren(IEnumerable<Child> children)
			{
				Children = children;
			}

			public IEnumerable<Child> Children { get; private set; }
			public string Name { get; set; }
		}

		private class Child
		{
			public Child(GrandChild grandChild)
			{
				GrandChild = grandChild;
			}

			public GrandChild GrandChild { get; private set; }
			public string Name { get; set; }
		}

		private class GrandChild { }

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