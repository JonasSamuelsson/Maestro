using Xunit;

namespace Maestro.Tests.Core.Diagnostics
{
	public class WhatDoIHave_tests
	{
		[Fact]
		public void WhatDoIHave()
		{
			var container = new Container(x => x.For<object>().Add.Type<object>())
				.GetChildContainer(x => x.For<string>().Use.Instance("child"));

			var s = container.Diagnostics.WhatDoIHave();
		}
	}
}