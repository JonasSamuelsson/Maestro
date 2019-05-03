using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Maestro.Microsoft.Hosting.Tests
{
	public class HostBuilderExtensionsTests
	{
		[Fact]
		public async Task ShouldHandleServicesRegisteredWithConfigureContainer()
		{
			var host = new HostBuilder()
				.UseMaestro()
				.ConfigureContainer(builder =>
				{
					builder.Add<object>().Instance("success");
					builder.Add<IFoo>().Type<Foo>();
				})
				.Build();

			host.Services.GetService<IFoo>().Dependency.ShouldBe("success");
		}

		[Fact]
		public async Task ShouldHandleServicesRegisteredWithConfigureServices()
		{
			var host = new HostBuilder()
				.UseMaestro()
				.ConfigureServices(services =>
				{
					services.AddSingleton<object>("success");
					services.AddTransient<IFoo, Foo>();
				})
				.Build();

			host.Services.GetService<IFoo>().Dependency.ShouldBe("success");
		}

		[Fact]
		public async Task ShouldHandleServicesRegisteredWithConfigureContainerAndConfigureServices()
		{
			var host = new HostBuilder()
				.UseMaestro()
				.ConfigureContainer(builder =>
				{
					builder.Add<object>().Instance("success");
				})
				.ConfigureServices(services =>
				{
					services.AddTransient<IFoo, Foo>();
				})
				.Build();

			host.Services.GetService<IFoo>().Dependency.ShouldBe("success");
		}

		private interface IFoo
		{
			object Dependency { get; set; }
		}

		private class Foo : IFoo
		{
			public Foo(object dependency)
			{
				Dependency = dependency;
			}

			public object Dependency { get; set; }
		}
	}
}
