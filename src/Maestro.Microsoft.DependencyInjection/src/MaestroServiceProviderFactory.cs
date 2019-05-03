using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	public class MaestroServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
	{
		public ContainerBuilder CreateBuilder(IServiceCollection services)
		{
			var builder = new ContainerBuilder();
			builder.Populate(services);
			return builder;
		}

		public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
		{
			return containerBuilder.BuildContainer().ToServiceProvider();
		}
	}
}