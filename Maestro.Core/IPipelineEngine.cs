using System;

namespace Maestro
{
	internal interface IPipelineEngine
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IPipelineEngine MakeGenericPipelineEngine(Type[] types);
		void SetLifecycle(ILifecycle lifecycle);
	}
}