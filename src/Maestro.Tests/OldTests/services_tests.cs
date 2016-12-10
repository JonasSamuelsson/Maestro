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
				x.Services<object>().Add.Instance("success");
			});

			container.GetServices<object>().ShouldBe(new[] { "success" });
		}

		[Fact]
		public void should_not_include_default_or_named_instance()
		{
			var container = new Container(x =>
			{
				x.Service<object>().Use.Instance("fail");
				x.Service<object>("foobar").Use.Instance("fail");
				x.Services<object>().Add.Instance("success");
			});

			container.GetServices<object>().ShouldBe(new[] { "success" });
		}
	}
}
