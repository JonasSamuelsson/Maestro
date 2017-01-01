using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class Self_type_registration_tests
	{
		[Fact]
		public void should_handle_non_generic_self_type_registration()
		{
			var container = new Container(x => x.Service(typeof(object)).Use.Self());

			container.GetService(typeof(object)).GetType().ShouldBe(typeof(object));
		}

		[Fact]
		public void should_handle_generic_self_type_registration()
		{
			var container = new Container(x => x.Service<object>().Use.Self());

			container.GetService<object>().GetType().ShouldBe(typeof(object));
		}
	}
}