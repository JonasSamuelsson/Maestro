using Maestro.Internals;
using System;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class DependencyDepthCheckerTests
	{
		[Fact]
		public void ExceedingMaxDepthShouldThrow()
		{
			var ddc = new DependencyDepthChecker(1);
			ddc.Push();
			Assert.Throws<InvalidOperationException>(() => ddc.Push());
		}

		[Fact]
		public void ShouldHandlePushAndPop()
		{
			var ddc = new DependencyDepthChecker(2);
			ddc.Push();
			ddc.Pop();
			ddc.Push();
			ddc.Push();
			ddc.Pop();
			ddc.Push();
			Assert.Throws<InvalidOperationException>(() => ddc.Push());
		}
	}
}
