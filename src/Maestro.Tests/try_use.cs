﻿using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class try_use
	{
		[Fact]
		public void should_not_throw_if_registering_same_default_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.Service<string>().TryUse.Instance("success");
				x.Service<string>().TryUse.Instance("fail");
			});

			container.Get<string>().ShouldBe("success");
		}

		[Fact]
		public void should_not_throw_if_registering_same_named_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.Service(typeof(string), "foobar").TryUse.Instance("success");
				x.Service(typeof(string), "foobar").TryUse.Instance("fail");
			});

			container.Get(typeof(string), "foobar").ShouldBe("success");
		}

		[Fact]
		public void TryUse_should_return_null_when_service_is_already_registered()
		{
			new Container(x =>
			{
				x.Service<object>().TryUse.Type<object>().ShouldNotBeNull();
				x.Service<object>().TryUse.Type<object>().ShouldBeNull();
			});
		}
	}
}