using System.Collections.Generic;
using System.Linq;
using Shouldly;
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
				x.Service<Parent>().Use.Type<Parent>();
				x.Services<IChild>().Add.Type<Child1>();
				x.Services<IChild>().Add.Type<Child2>();
				x.Services<IChild>().Add.Type<Child3>();
			});

			var parent = container.GetService<Parent>();

			parent.Children.Count().ShouldBe(3);
			parent.Children.ShouldContain(x => x.GetType() == typeof(Child1));
			parent.Children.ShouldContain(x => x.GetType() == typeof(Child2));
			parent.Children.ShouldContain(x => x.GetType() == typeof(Child3));
		}

		[Fact]
		public void should_use_empty_enumerable_if_enumerated_type_is_not_registered()
		{
			var container = new Container();

			var foobar = container.GetService<Parent>();

			foobar.Children.ShouldNotBeNull();
			foobar.Children.ShouldBeEmpty();
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