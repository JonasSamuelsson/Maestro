using System;

namespace Maestro
{
	internal class ServiceProvider : IServiceProvider
	{
		private readonly TryGetService _tryGetService;

		public ServiceProvider(TryGetService tryGetService)
		{
			_tryGetService = tryGetService;
		}

		public object GetService(Type serviceType)
		{
			_tryGetService(serviceType, out var instance);
			return instance;
		}

		internal delegate bool TryGetService(Type serviceType, out object instance);
	}
}