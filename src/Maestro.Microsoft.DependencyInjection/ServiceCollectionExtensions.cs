using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMaestro(this IServiceCollection services)
		{
			services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new MaestroServiceProviderFactory());

			return services;
		}
	}
}