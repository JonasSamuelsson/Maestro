using Shouldly;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Maestro.Tests.Core
{
	public class ChildContainer_resolution_tests
	{
		[Fact]
		public void should_prefer_generic_child_container_config_over_specific_parent_config()
		{
			var parentContainer = new Container(x => x.Use<ICollection<object>>().Type<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.Use(typeof(ICollection<>)).Type(typeof(List<>)));

			var collection = childContainer.GetService<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}

		[Fact]
		public void GetServices_should_resolve_instances_from_both_child_and_parent_container()
		{
			var container = new Container(x => x.Add<object>().Instance("parent"))
				.GetChildContainer(x => x.Add<object>().Instance("child"));

			container.GetServices<object>().ShouldBe(new[] { "child", "parent" });
		}

		[Fact]
		public void GetServices_should_not_resolve_from_parent_if_enumerable_instance_is_registered_in_child()
		{
			var container = new Container(x => x.Add<object>().Instance("fail"))
				.GetChildContainer(x =>
				{
					x.Add<object>().Instance("fail");
					x.Use<IEnumerable<object>>().Instance(new[] { "1" });
				})
				.GetChildContainer(x => x.Add<object>().Instance("2"));

			container.GetServices<object>().ShouldBe(new[] { "2", "1" });
		}
	}
}
