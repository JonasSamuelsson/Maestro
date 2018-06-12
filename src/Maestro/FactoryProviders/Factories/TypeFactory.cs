using System;
using Maestro.Diagnostics;

namespace Maestro.FactoryProviders.Factories
{
	internal class TypeFactory : Factory
	{
		private readonly Type _type;
		private readonly Func<Context, object> _activator;

		internal TypeFactory(Type type, Func<Context, object> activator)
		{
			_type = type;
			_activator = activator;
		}

		internal override object GetInstance(Context context)
		{
			return _activator(context);
		}

		internal override void Populate(PipelineService service)
		{
			service.InstanceType = _type;
			service.Provider = "Type";
		}
	}
}