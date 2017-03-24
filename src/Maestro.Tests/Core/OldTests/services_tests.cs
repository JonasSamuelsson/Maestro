using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class services_tests
	{
		[Fact]
		public void should_get_services()
		{
			var container = new Container(x =>
			{
				x.For<object>().Use.Instance("1");
				x.For<object>("foobar").Use.Instance("2");
				x.For<object>().Add.Instance("3");
			});

			container.GetServices<object>().ShouldBe(new[] { "1", "2", "3" });
		}
	}
}
