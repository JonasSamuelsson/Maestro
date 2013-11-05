using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_dependencies
	{
		[Fact]
		public void should_get_instances_with_same_name_as_requested_instance_or_fall_back_to_default_instance()
		{
			var @default = "default";
			var name1 = "name1";
			var name2 = "name2";
			var defaultGrandChild = new GrandChild();
			var namedGrandChild = new GrandChild();
			var container = new Container(x =>
			{
				x.For<ParentWithSingleChild>().Use<ParentWithSingleChild>().Set(y => y.Tag, @default);
				x.For<ParentWithSingleChild>(name1).Use<ParentWithSingleChild>().Set(y => y.Tag, name1);
				x.For<ParentWithMultipleChildren>().Use<ParentWithMultipleChildren>().Set(y => y.Tag, @default);
				x.For<ParentWithMultipleChildren>(name1).Use<ParentWithMultipleChildren>().Set(y => y.Tag, name1);
				x.For<Child>().Use<Child>().Set(y => y.Tag, @default);
				x.For<Child>(name2).Use<Child>().Set(y => y.Tag, name2);
				x.For<GrandChild>().Use(defaultGrandChild);
				x.For<GrandChild>(name1).Use(namedGrandChild);
			});

			{
				var singleParent = container.Get<ParentWithSingleChild>();
				singleParent.Tag.Should().Be(@default);
				singleParent.Child.Tag.Should().Be(@default);
				singleParent.Child.GrandChild.Should().Be(defaultGrandChild);

				var singleParent1 = container.Get<ParentWithSingleChild>(name1);
				singleParent1.Tag.Should().Be(name1);
				singleParent1.Child.Tag.Should().Be(@default);
				singleParent1.Child.GrandChild.Should().Be(namedGrandChild);

				var singleParent2 = container.Get<ParentWithSingleChild>(name2);
				singleParent2.Tag.Should().Be(@default);
				singleParent2.Child.Tag.Should().Be(name2);
				singleParent2.Child.GrandChild.Should().Be(@defaultGrandChild);
			}

			{
				var multiParent = container.Get<ParentWithMultipleChildren>();
				multiParent.Tag.Should().Be(@default);
				multiParent.Children.Should().HaveCount(2);
				multiParent.Children.Should().OnlyContain(x => x.GrandChild == defaultGrandChild);

				var multiParent1 = container.Get<ParentWithMultipleChildren>(name1);
				multiParent1.Tag.Should().Be(name1);
				multiParent1.Children.Should().HaveCount(2);
				multiParent1.Children.Should().OnlyContain(x => x.GrandChild == namedGrandChild);

				var multiParent2 = container.Get<ParentWithMultipleChildren>(name2);
				multiParent2.Tag.Should().Be(@default);
				multiParent2.Children.Should().HaveCount(2);
				multiParent2.Children.Should().OnlyContain(x => x.GrandChild == defaultGrandChild);
			}
		}

		[Fact]
		public void should_get_dependencies_with_same_name_as_top_instance_or_fall_back_to_default_instance()
		{
			var name1 = "abc";
			var name2 = "xyz";
			var grandChild = new GrandChild();
			var grandChild1 = new GrandChild();
			var container = new Container(x =>
			{
				x.For<ParentWithSingleChild>().Use<ParentWithSingleChild>().Set(y => y.Tag, grandChild);
				x.For<ParentWithSingleChild>(name1).Use<ParentWithSingleChild>().Set(y => y.Tag, grandChild1);
				x.For<ParentWithSingleChild>(name2).Use<ParentWithSingleChild>().Set(y => y.Tag, grandChild);
				x.For<GrandChild>().Use(grandChild);
				x.For<GrandChild>(name1).Use(grandChild1);
			});

			container.GetAll<ParentWithSingleChild>().Should().OnlyContain(x => x.Tag == x.Child.GrandChild);
		}

		private class ParentWithSingleChild
		{
			public ParentWithSingleChild(Child child)
			{
				Child = child;
			}

			public Child Child { get; private set; }
			public object Tag { get; set; }
		}

		private class ParentWithMultipleChildren
		{
			public ParentWithMultipleChildren(IEnumerable<Child> children)
			{
				Children = children;
			}

			public IEnumerable<Child> Children { get; private set; }
			public object Tag { get; set; }
		}

		private class Child
		{
			public Child(GrandChild grandChild)
			{
				GrandChild = grandChild;
			}

			public GrandChild GrandChild { get; private set; }
			public object Tag { get; set; }
		}

		private class GrandChild { }
	}
}