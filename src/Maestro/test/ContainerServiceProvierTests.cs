using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class ContainerServiceProvierTests
	{
		[Fact]
		public void ShouldGetService()
		{
			var container = new Container(x => x.Add<object>().Instance("success"));
			var serviceProvider = container.ToServiceProvider();
			serviceProvider.GetService(typeof(object)).ShouldBe("success");
		}

		[Fact]
		public void ShouldGetServices()
		{
			var container = new Container(x => x.Add<object>().Instance("success"));
			var serviceProvider = container.ToServiceProvider();
			serviceProvider.GetService(typeof(IEnumerable<object>)).ShouldBe(new[] { "success" });
		}

		[Fact]
		public void ShouldGetNullForUnregisteredService()
		{
			var container = new Container();
			var serviceProvider = container.ToServiceProvider();
			serviceProvider.GetService(typeof(string)).ShouldBeNull();
		}
	}
}