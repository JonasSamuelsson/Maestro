using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	internal sealed class MaestroServiceScopeFactory : IServiceScopeFactory
	{
		private readonly IScopedContainer _container;

		public MaestroServiceScopeFactory(IScopedContainer container)
		{
			_container = container;
		}

		public IServiceScope CreateScope()
		{
			return new MaestroServiceScope(_container.CreateScope());
		}
	}
}