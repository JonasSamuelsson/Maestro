using System;
using Maestro.Interceptors;
using Maestro.Lifecycles;

namespace Maestro
{
	internal interface IPipelineEngine
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IPipelineEngine MakeGenericPipelineEngine(Type[] types);
		void AddOnCreateInterceptor(IInterceptor interceptor);
		void SetLifecycle(ILifecycle lifecycle);
		void AddOnActivateInterceptor(IInterceptor interceptor);
	}
}