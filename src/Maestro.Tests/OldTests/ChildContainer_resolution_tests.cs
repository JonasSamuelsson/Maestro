using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ChildContainer_resolution_tests
	{
		[Fact]
		public void should_prefer_generic_child_container_config_over_specific_parent_config()
		{
			var parentContainer = new Container(x => x.For<ICollection<object>>().Use.Type<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.For(typeof(ICollection<>)).Use.Type(typeof(List<>)));

			var collection = childContainer.GetService<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}

		[Fact]
		public void GetServices_should_resolve_instances_from_both_child_and_parent_container()
		{
			var container = new Container(x => x.For<object>().Add.Instance("parent"))
				.GetChildContainer(x => x.For<object>().Add.Instance("child"));

			container.GetServices<object>().ShouldBe(new[] { "child", "parent" });
		}

		[Fact]
		public void GetServices_should_not_resolve_from_parent_if_enumerable_is_registered()
		{
			var container = new Container(x => x.For<object>().Add.Instance("fail"))
				.GetChildContainer(x =>
				{
					x.For<object>().Add.Instance("fail");
					x.For<IEnumerable<object>>().Use.Instance(new[] { "1" });
				})
				.GetChildContainer(x => x.For<object>().Add.Instance("2"));

			container.GetServices<object>().ShouldBe(new[] { "2", "1" }); // todo - wip
		}
	}
}
