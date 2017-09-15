using System;
using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	public class MaestroServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
	{
		private readonly ContainerBuilder _builder;

		public MaestroServiceProviderFactory(ContainerBuilder builder)
		{
			_builder = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public ContainerBuilder CreateBuilder(IServiceCollection services)
		{
			_builder.Populate(services);
			return _builder;
		}

		public IServiceProvider CreateServiceProvider(ContainerBuilder builder)
		{
			return new MaestroServiceProvider(new Container(builder));
		}
	}
}