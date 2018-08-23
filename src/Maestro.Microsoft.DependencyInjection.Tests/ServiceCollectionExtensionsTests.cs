using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Maestro.Microsoft.DependencyInjection.Tests
{
	public class ServiceCollectionExtensionsTests
	{
		[Fact]
		public void ShouldAddServiceProviderFactory()
		{
			var serviceCollection = new ServiceCollection()
				.AddMaestro();

			serviceCollection.ShouldContain(x => x.ServiceType == typeof(IServiceProviderFactory<IContainerBuilder>));
			serviceCollection.ShouldContain(x => x.ServiceType == typeof(IServiceProviderFactory<ContainerBuilder>));
		}
	}
}