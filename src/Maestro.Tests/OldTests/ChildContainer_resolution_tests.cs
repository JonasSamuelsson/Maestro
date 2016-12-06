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
			var parentContainer = new Container(x => x.Service<ICollection<object>>().Use.Type<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.Service(typeof(ICollection<>)).Use.Type(typeof(List<>)));

			var collection = childContainer.GetService<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}

		[Fact]
		public void GetAll_should_get_instances_from_parent_container_if_name_is_different()
		{
			var parentContainer = new Container(x =>
			{
				x.Service<object>().Use.Instance("default parent");
				x.Services<object>().Add.Instance("parent");
			});

			var childContainer = parentContainer.GetChildContainer(x =>
			{
				x.Service<object>().Use.Instance("default child");
				x.Services<object>().Add.Instance("child");
			});

			childContainer.GetServices<object>().ShouldBe(new[] { "default child", "child", "parent" });
		}
	}
}
