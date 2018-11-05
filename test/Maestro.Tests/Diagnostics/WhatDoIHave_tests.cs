using Maestro.Configuration;
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
				x.Add<string>().Instance("child");
			});

			container.GetService<string>();
			container.GetService<AutoResolved>();

			var s = container.Diagnostics.WhatDoIHave();
		}

		[Fact]
		public void ResolveDiagnostics()
		{
			var container = new Container(x =>
			{
				x.Add<object>().Type<object>();
				x.Add<string>().Instance("child");
				x.AddDiagnostics();
			});

			container.GetService<string>();
			container.GetService<AutoResolved>();

			var s = container.GetService<Maestro.Diagnostics.Diagnostics>().WhatDoIHave();
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class AutoResolved { }
	}
}