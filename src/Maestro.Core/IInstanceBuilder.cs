using Maestro.Interceptors;
using Maestro.Lifetimes;
using System;
using Maestro.Utils;

namespace Maestro
{
	internal interface IInstanceBuilder
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IInstanceBuilder MakeGenericPipelineEngine(Type[] types);
		void SetLifetime(ILifetime lifetime);
		void AddInterceptor(IInterceptor interceptor);
		void GetConfiguration(DiagnosticsBuilder builder);
		IInstanceBuilder Clone();
	}
}