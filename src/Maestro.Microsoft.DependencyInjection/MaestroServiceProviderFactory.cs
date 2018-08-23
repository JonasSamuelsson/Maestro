using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	public class MaestroServiceProviderFactory :IServiceProviderFactory<IContainerBuilder>, IServiceProviderFactory<ContainerBuilder>
	{
		IContainerBuilder IServiceProviderFactory<IContainerBuilder>.CreateBuilder(IServiceCollection services)
		{
			return CreateBuilder(services);
		}

		public IServiceProvider CreateServiceProvider(IContainerBuilder containerBuilder)
		{
			return CreateServiceProvider((ContainerBuilder)containerBuilder);
		}

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