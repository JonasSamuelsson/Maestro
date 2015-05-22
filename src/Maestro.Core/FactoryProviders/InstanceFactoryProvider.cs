using System;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly object _instance;

		public InstanceFactoryProvider(object instance)
		{
			_instance = instance;
		}

		public IFactory GetFactory(Context context)
		{
			return new InstanceFactory(_instance);
		}
	}
}