using System;

namespace Maestro.Microsoft.DependencyInjection
{
	internal class MaestroServiceProvider : IServiceProvider
	{
		private readonly IContext _context;

		public MaestroServiceProvider(IContext context)
		{
			_context = context;
		}

		public object GetService(Type serviceType)
		{
			object instance;
			return _context.TryGetService(serviceType, out instance)
				? instance
				: null;
		}
	}
}