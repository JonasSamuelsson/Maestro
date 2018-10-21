using Maestro.Configuration;
using Maestro.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Maestro.Microsoft.Hosting
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder ConfigureContainer(this IHostBuilder hostBuilder, Action<ContainerBuilder> configureDelegate)
		{
			return hostBuilder.ConfigureContainer((context, containerBuilder) => configureDelegate.Invoke(containerBuilder));
		}

		public static IHostBuilder ConfigureContainer(this IHostBuilder hostBuilder, Action<HostBuilderContext, ContainerBuilder> configureDelegate)
		{
			return hostBuilder.ConfigureContainer<ContainerBuilder>(configureDelegate.Invoke);
		}

		public static IHostBuilder UseMaestro(this IHostBuilder hostBuilder)
		{
			return hostBuilder.UseServiceProviderFactory(new MaestroServiceProviderFactory());
		}
	}
}
