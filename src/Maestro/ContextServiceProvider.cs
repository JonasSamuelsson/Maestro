using System;

namespace Maestro
{
	internal class ContextServiceProvider : IServiceProvider
	{
		private readonly Context _context;

		public ContextServiceProvider(Context context)
		{
			_context = context;
		}

		public object GetService(Type serviceType)
		{
			// ReSharper disable once ConditionalTernaryEqualBranch
			return _context.TryGetService(serviceType, out var instance) ? instance : instance;
		}
	}
}