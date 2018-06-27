using Microsoft.Extensions.DependencyInjection;
using System;

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

		private class MaestroServiceScope : IServiceScope
		{
			private readonly IScopedContainer _container;

			public MaestroServiceScope(IScopedContainer container)
			{
				_container = container;
				ServiceProvider = container.GetService<IServiceProvider>();
			}

			public IServiceProvider ServiceProvider { get; }

			public void Dispose() => _container.Dispose();
		}
	}
}