using Maestro.Diagnostics;

namespace Maestro.FactoryProviders.Factories
{
	internal class InstanceFactory : Factory
	{
		private readonly object _instance;

		internal InstanceFactory(object instance)
		{
			_instance = instance;
		}

		internal override object GetInstance(Context context)
		{
			return _instance;
		}

		internal override void Populate(PipelineService service)
		{
			service.InstanceType = _instance.GetType();
			service.Provider = "Instance";
		}
	}
}