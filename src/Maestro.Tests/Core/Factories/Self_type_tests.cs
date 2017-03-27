using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Factories
{
	public class Self_type_registration_tests
	{
		[Fact]
		public void should_handle_non_generic_self_type_registration()
		{
			var container = new Container(x => x.For(typeof(object)).Use.Self());

			container.GetService(typeof(object)).GetType().ShouldBe(typeof(object));
		}

		[Fact]
		public void should_handle_generic_self_type_registration()
		{
			var container = new Container(x => x.For<object>().Use.Self());

			container.GetService<object>().GetType().ShouldBe(typeof(object));
		}
	}
}