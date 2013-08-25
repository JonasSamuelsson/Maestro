using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class singleton_lifecycle
	{
		[Fact]
		public void sinlgeton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.For<object>().Use<object>().Lifecycle.Singleton());

			var o1 = container.Get<object>();
			var o2 = container.Get<object>();

			o1.Should().Be(o2);
		}

	}
}