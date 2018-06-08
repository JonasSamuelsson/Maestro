using System;

namespace Maestro.FactoryProviders.Factories
{
	class Factory : IFactory
	{
		private readonly Func<Context, object> _activator;

		public Factory(Func<Context, object> activator)
		{
			_activator = activator;
		}

		public object GetInstance(Context context)
		{
			return _activator(context);
		}
	}
}