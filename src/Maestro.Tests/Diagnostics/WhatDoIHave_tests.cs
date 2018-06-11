using Xunit;

namespace Maestro.Tests.Diagnostics
{
	public class WhatDoIHave_tests
	{
		[Fact]
		public void WhatDoIHave()
		{
			var container = new Container(x =>
			{
				x.Add<object>().Type<object>();
				x.Use<string>().Instance("child");
			});

			var s = container.Diagnostics.WhatDoIHave();
		}
	}
}