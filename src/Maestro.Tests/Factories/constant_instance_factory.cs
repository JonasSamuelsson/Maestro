using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class constant_instance_factory
	{
		[Fact]
		public void should_use_provided_instance()
		{
			var instance = "default instance";
			var container = new Container(x => x.Service<object>().Use.Instance(instance));

			var o = container.Get<object>();

			o.Should().Be(instance);
		}
	}
}