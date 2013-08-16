using FluentAssertions;
using System;
using Xunit;

namespace Maestro.Tests
{
	public class lambda_instance
	{
		[Fact]
		public void should_delegate_instantiation_to_provided_lambda()
		{
			var name = "foo";
			var container = new Container(x =>
			{
				x.For<object>().Use(() => new EventArgs());
				x.Add<object>(name).Use(_ => new Exception());
			});

			var @default = container.Get<object>();
			var named = container.Get<object>(name);

			@default.Should().BeOfType<EventArgs>();
			named.Should().BeOfType<Exception>();
		}
	}
}