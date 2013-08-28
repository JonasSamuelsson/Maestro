using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_enumerable_dependencies
	{
		[Fact]
		public void should_use_all_registered_instances_of_the_enumerated_type()
		{
			var container = new Container(x =>
			{
				x.For<Parent>().Use<Parent>();
				x.Add<IChild>().Use<Child1>();
				x.Add<IChild>().Use<Child2>();
				x.Add<IChild>().Use<Child3>();
			});

			var parent = container.Get<Parent>();

			parent.Children.Should().HaveCount(3);
			parent.Children.Should().Contain(x => x.GetType() == typeof(Child1));
			parent.Children.Should().Contain(x => x.GetType() == typeof(Child2));
			parent.Children.Should().Contain(x => x.GetType() == typeof(Child3));
		}

		[Fact]
		public void should_use_empty_enumerable_if_enumerated_type_is_not_registered()
		{
			var container = new Container();

			var foobar = container.Get<Parent>();

			foobar.Children.Should().NotBeNull();
			foobar.Children.Should().HaveCount(0);
		}

		private class Parent
		{
			public Parent(IEnumerable<IChild> children) { Children = children; }
			public IEnumerable<object> Children { get; private set; }
		}
		private interface IChild { }
		private class Child1 : IChild { }
		private class Child2 : IChild { }
		private class Child3 : IChild { }
	}
}