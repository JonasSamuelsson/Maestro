using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class default_lifecycle
	{
		[Fact]
		public void should_be_applied_to_all_instances()
		{
			var container = new Container(x =>
													{
														x.Default.Lifecycle.Singleton();
														x.For<object>().Use<object>();
													});
			var instance1 = container.Get<object>();
			var instance2 = container.Get<object>();

			instance1.Should().Be(instance2);
		}
	}
}