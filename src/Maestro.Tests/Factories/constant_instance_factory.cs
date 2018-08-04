using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class constant_instance_factory
	{
		[Fact]
		public void should_use_provided_instance()
		{
			var instance = "default instance";
			var container = new Container(x => x.Add<object>().Instance(instance));

			var o = container.GetService<object>();

			o.ShouldBe(instance);
		}
	}
}