using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMaestro(this IServiceCollection services)
		{
			return AddMaestro(services, new ContainerBuilder());
		}

		public static IServiceCollection AddMaestro(this IServiceCollection services, ContainerBuilder builder)
		{
			return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new MaestroServiceProviderFactory(builder));
		}
	}
}