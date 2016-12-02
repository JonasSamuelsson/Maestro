using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class singleton_lifetime
	{
		[Fact]
		public void singleton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.Service<object>().Use.Type<object>().Lifetime.Singleton());

			var o1 = container.GetService<object>();
			var o2 = container.GetService<object>();

			o1.Should().Be(o2);
		}
	}
}