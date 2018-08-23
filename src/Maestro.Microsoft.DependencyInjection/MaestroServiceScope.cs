using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	internal class MaestroServiceScope : IServiceScope
	{
		private readonly IScopedContainer _container;

		public MaestroServiceScope(IScopedContainer container)
		{
			_container = container;
		}

		public IServiceProvider ServiceProvider => _container.ToServiceProvider();

		public void Dispose() => _container.Dispose();
	}
}