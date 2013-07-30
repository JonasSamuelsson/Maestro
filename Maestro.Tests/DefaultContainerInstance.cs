using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class DefaultContainerInstance
	{
		[Fact]
		public void should_not_be_null()
		{
			Container.Default.Should().NotBeNull();
		}

		[Fact]
		public void should_always_return_the_same_instance()
		{
			var container1 = Container.Default;
			var container2 = Container.Default;

			container1.Should().Be(container2);
		}
	}
}