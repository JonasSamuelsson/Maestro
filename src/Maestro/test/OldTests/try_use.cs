﻿using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class try_use
	{
		[Fact]
		public void should_not_throw_if_registering_same_default_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.TryAdd<string>().Instance("success");
				x.TryAdd<string>()?.Instance("fail");
			});

			container.GetService<string>().ShouldBe("success");
		}

		[Fact]
		public void should_not_throw_if_registering_same_named_service_twice()
		{
			var container = new Container();

			container.Configure(x =>
			{
				x.TryAdd(typeof(string))?.Named("foobar").Instance("success");
				x.TryAdd(typeof(string))?.Named("foobar").Instance("fail");
			});

			container.GetService(typeof(string), "foobar").ShouldBe("success");
		}

		[Fact]
		public void TryUse_should_return_null_when_service_is_already_registered()
		{
			new Container(x =>
			{
				x.TryAdd<object>().Type<object>().ShouldNotBeNull();
				x.TryAdd<object>().ShouldBeNull();
			});
		}
	}
}