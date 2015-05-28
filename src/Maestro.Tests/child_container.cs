using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class child_container
	{
		[Fact]
		public void should_be_able_to_override_root_container_config()
		{
			var rootContainer = new Container(x => x.For<object>().Use<object>());

			var o = new object();
			var childContainer = rootContainer.GetChildContainer(x => x.For<object>().Use(o));

			var rootInstance = rootContainer.Get<object>();
			var childInstance = childContainer.Get<object>();

			rootInstance.ShouldNotBe(childInstance);
			childInstance.ShouldBe(o);
		}
	}
}