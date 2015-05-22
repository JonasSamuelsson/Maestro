using Maestro.Internals;

namespace Maestro.FactoryProviders.Factories
{
	class InstanceFactory : IFactory
	{
		private readonly object _instance;

		public InstanceFactory(object instance)
		{
			_instance = instance;
		}

		public object GetInstance(Context context)
		{
			return _instance;
		}
	}
}