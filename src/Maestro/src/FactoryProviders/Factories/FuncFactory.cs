using System;
using Maestro.Diagnostics;

namespace Maestro.FactoryProviders.Factories
{
	internal class FuncFactory : Factory
	{
		private readonly Func<Context, object> _activator;

		internal FuncFactory(Func<Context, object> activator)
		{
			_activator = activator;
		}

		internal override object GetInstance(Context context)
		{
			return _activator(context);
		}

		internal override void Populate(PipelineService service)
		{
			service.Provider = "Factory";
		}
	}
}