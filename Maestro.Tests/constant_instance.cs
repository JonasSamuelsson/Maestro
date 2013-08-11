using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class constant_instance
	{
		[Fact]
		public void should_use_provided_instance()
		{
			var instance = "default object";
			var container = new Container(x => x.Default<object>().Is(instance));

			var o = container.Get<object>();

			o.Should().Be(instance);
		} 
	}
}