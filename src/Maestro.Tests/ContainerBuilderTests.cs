using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ContainerBuilderTests
	{
		[Fact]
		public void ShouldConfigureContainer()
		{
			var builder = new ContainerBuilder(x => x.Use<IInterface>().Type<Class>());
			var container = new Container(builder);
			container.GetService<IInterface>().ShouldBeOfType<Class>();
		}

		private interface IInterface { }
		private class Class : IInterface { }
	}
}