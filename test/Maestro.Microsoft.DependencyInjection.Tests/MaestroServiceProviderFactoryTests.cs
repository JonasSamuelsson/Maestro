using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Maestro.Microsoft.DependencyInjection.Tests
{
	public class MaestroServiceProviderFactoryTests
	{
		[Fact]
		public void ShouldCreateServiceProvider()
		{
			var services = new ServiceCollection { new ServiceDescriptor(typeof(object), "success") };
			var factory = new MaestroServiceProviderFactory();
			var builder = factory.CreateBuilder(services);
			var serviceProvider = factory.CreateServiceProvider(builder);
			serviceProvider.GetService<object>().ShouldBe("success");
		}
	}
}