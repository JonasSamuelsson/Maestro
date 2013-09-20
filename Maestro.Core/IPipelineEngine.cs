using System;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro
{
	internal interface IPipelineEngine
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IPipelineEngine MakeGenericPipelineEngine(Type[] types);
		void AddOnCreateInterceptor(IInterceptor interceptor);
		void SetLifetime(ILifetime lifetime);
		void AddOnActivateInterceptor(IInterceptor interceptor);
		void GetConfiguration(DiagnosticsBuilder builder);
	}
}