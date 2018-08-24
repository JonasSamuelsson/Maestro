using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Configuration
{
	public class ContainerBuilderTests
	{
		[Fact]
		public void ShouldBuildContainer()
		{
			var builder = new ContainerBuilder();

			builder.Add<string>().Instance("success");

			var container = builder.BuildContainer();

			container.GetService<string>().ShouldBe("success");
		}

		[Fact]
		public void ShouldCreateNewContainerOnEachCallToBuildContainer()
		{
			var builder = new ContainerBuilder();

			builder.Add<object>().Self().Singleton();

			var c1 = builder.BuildContainer();
			var c2 = builder.BuildContainer();

			c1.ShouldNotBe(c2);

			var o1 = c1.GetService<object>();
			var o2 = c2.GetService<object>();

			o1.ShouldNotBe(o2);
		}
	}
}