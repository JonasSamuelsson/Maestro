using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	internal sealed class MaestroServiceScopeFactory : IServiceScopeFactory
	{
		private readonly IContainer _container;

		public MaestroServiceScopeFactory(IContainer container)
		{
			_container = container;
		}

		public IServiceScope CreateScope()
		{
			return new MaestroServiceScope(_container.CreateScope());
		}
	}
}