using System;
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
			return new MaestroServiceScope(_container.GetChildContainer());
		}

		private class MaestroServiceScope : IServiceScope
		{
			private readonly IContainer _container;

			public MaestroServiceScope(IContainer container)
			{
				_container = container;
				ServiceProvider = container.GetService<IServiceProvider>();
			}

			public IServiceProvider ServiceProvider { get; }

			public void Dispose() => _container.Dispose();
		}
	}
}