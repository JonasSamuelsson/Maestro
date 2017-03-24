using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class try_use
	{
		[Fact]
		public void should_not_throw_if_registering_same_default_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.For<string>().TryUse.Instance("success");
				x.For<string>().TryUse.Instance("fail");
			});

			container.GetService<string>().ShouldBe("success");
		}

		[Fact]
		public void should_not_throw_if_registering_same_named_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.For(typeof(string), "foobar").TryUse.Instance("success");
				x.For(typeof(string), "foobar").TryUse.Instance("fail");
			});

			container.GetService(typeof(string), "foobar").ShouldBe("success");
		}

		[Fact]
		public void TryUse_should_return_null_when_service_is_already_registered()
		{
			new Container(x =>
			{
				x.For<object>().TryUse.Type<object>().ShouldNotBeNull();
				x.For<object>().TryUse.Type<object>().ShouldBeNull();
			});
		}
	}
}