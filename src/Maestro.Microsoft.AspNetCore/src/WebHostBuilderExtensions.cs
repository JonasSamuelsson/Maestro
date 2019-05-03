using Maestro.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Maestro.Microsoft.AspNetCore
{
	public static class WebHostBuilderExtensions
	{
		public static IWebHostBuilder UseMaestro(this IWebHostBuilder webHostBuilder)
		{
			return webHostBuilder.ConfigureServices(services => services.AddMaestro());
		}
	}
}