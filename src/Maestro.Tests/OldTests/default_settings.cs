using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Conventions;
using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class default_settings
	{
		[Fact]
		public void default_lifetime_should_be_transient()
		{
			var container = new Container(x => x.Use<object>().Self());

			var instance1 = container.GetService<object>();
			var instance2 = container.GetService<object>();

			instance1.ShouldNotBe(instance2);
		}
	}
}