using System;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class ConditionalPipelineEngine : IPipelineEngine
	{
		private readonly IProvider _provider;

		public ConditionalPipelineEngine(IProvider provider)
		{
			_provider = provider;
		}

		public bool CanGet(IContext context)
		{
			return _provider.CanGet(context);
		}

		public object Get(IContext context)
		{
			return _provider.Get(context);
		}

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			var genericProvider = _provider.MakeGenericProvider(types);
			return _provider is ConditionalInstanceProvider
				? (IPipelineEngine)new ConditionalPipelineEngine(genericProvider)
				: new PipelineEngine(genericProvider);
		}

		public void AddOnCreateInterceptor(IInterceptor interceptor)
		{
			throw new NotSupportedException();
		}

		public void SetLifetime(ILifetime lifetime)
		{
			throw new NotSupportedException();
		}

		public void AddOnActivateInterceptor(IInterceptor interceptor)
		{
			throw new NotSupportedException();
		}
	}
}