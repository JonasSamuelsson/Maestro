using System;

namespace Maestro.FactoryProviders.Factories
{
	internal class DelegatingFactory : Factory
	{
		private readonly Func<Context, object> _activator;

		public DelegatingFactory(Func<Context, object> activator)
		{
			_activator = activator;
		}

		internal override object GetInstance(Context context)
		{
			return _activator(context);
		}
	}
}