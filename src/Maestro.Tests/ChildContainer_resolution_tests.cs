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
			var parentContainer = new Container(x => x.For<ICollection<object>>().Use<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.For(typeof(ICollection<>)).Use(typeof(List<>)));

			var collection = childContainer.Get<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}
	}
}
