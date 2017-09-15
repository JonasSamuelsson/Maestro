using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class ContainerBuilderTests
	{
		[Fact]
		public void ShouldConfigureContainer()
		{
			var builder = new ContainerBuilder(x => x.For<IInterface>().Use.Type<Class>());
			var container = new Container(builder);
			container.GetService<IInterface>().ShouldBeOfType<Class>();
		}

		private interface IInterface { }
		private class Class : IInterface { }
	}
}