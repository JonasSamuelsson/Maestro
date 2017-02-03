using System;

namespace Maestro.FactoryProviders.Factories
{
	class Factory : IFactory
	{
		private readonly Func<IContext, object> _activator;

		public Factory(Func<IContext, object> activator)
		{
			_activator = activator;
		}

		public object GetInstance(IContext context)
		{
			return _activator(context);
		}
	}
}