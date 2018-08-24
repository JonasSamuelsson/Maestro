using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	public class MaestroServiceProviderFactory : IServiceProviderFactory<IContainerBuilder>, IServiceProviderFactory<Configuration.ContainerBuilder>, IServiceProviderFactory<ContainerBuilder>
	{
		IContainerBuilder IServiceProviderFactory<IContainerBuilder>.CreateBuilder(IServiceCollection services)
		{
			return CreateBuilder(services);
		}

		public IServiceProvider CreateServiceProvider(IContainerBuilder containerBuilder)
		{
			return ((Configuration.ContainerBuilder)containerBuilder).BuildContainer().ToServiceProvider();
		}

		public Configuration.ContainerBuilder CreateBuilder(IServiceCollection services)
		{
			var builder = new Configuration.ContainerBuilder();
			builder.Populate(services);
			return builder;
		}

		public IServiceProvider CreateServiceProvider(Configuration.ContainerBuilder containerBuilder)
		{
			return containerBuilder.BuildContainer().ToServiceProvider();
		}

		ContainerBuilder IServiceProviderFactory<ContainerBuilder>.CreateBuilder(IServiceCollection services)
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