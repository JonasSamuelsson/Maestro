using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class services_tests
	{
		[Fact]
		public void should_get_services()
		{
			var container = new Container(x =>
			{
				x.Use<object>().Instance("1");
				x.Use<object>("foobar").Instance("2");
				x.Add<object>().Instance("3");
			});

			container.GetServices<object>().OrderBy(s => s).ShouldBe(new[] { "1", "2", "3" });
		}
	}
}
