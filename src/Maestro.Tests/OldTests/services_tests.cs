using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class services_tests
	{
		[Fact]
		public void should_get_services()
		{
			var @object = new object();

			var container = new Container(x =>
			{
				x.Service<object>().Use.Type<object>();
				x.Services<object>().Add.Instance(@object);
			});

			container.GetServices<object>().ShouldBe(new[] { @object });
		}
	}
}
