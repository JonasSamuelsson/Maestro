﻿using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class constant_instance_provider
	{
		[Fact]
		public void should_use_provided_instance()
		{
			var instance = "default instance";
			var container = new Container(x => x.For<object>().Use(instance));

			var o = container.Get<object>();

			o.Should().Be(instance);
		}
	}
}