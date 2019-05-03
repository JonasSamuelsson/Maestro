using Maestro.Diagnostics;

namespace Maestro.FactoryProviders.Factories
{
	internal abstract class Factory
	{
		internal abstract object GetInstance(Context context);
		internal abstract void Populate(PipelineService service);
	}
}