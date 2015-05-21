using System;

namespace Maestro.Internals.FactoryProviders
{
	class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _providerMethod;

		public InstanceFactoryProvider(object instance)
		{
			_providerMethod = _ => instance;
		}

		public Func<Context, object> GetFactory(Context context)
		{
			return _providerMethod;
		}
	}
}