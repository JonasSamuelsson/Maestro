using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Lifetimes
{
	public class singleton_lifetime
	{
		[Fact]
		public void singleton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.For<object>().Use.Type<object>().Lifetime.Singleton());

			var o1 = container.GetService<object>();
			var o2 = container.GetService<object>();

			o1.ShouldBe(o2);
		}
	}
}