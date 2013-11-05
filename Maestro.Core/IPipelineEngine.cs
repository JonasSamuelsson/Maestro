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
		void SetLifetime(ILifetime lifetime);
		void AddInterceptor(IInterceptor interceptor);
		void GetConfiguration(DiagnosticsBuilder builder);
	}
}