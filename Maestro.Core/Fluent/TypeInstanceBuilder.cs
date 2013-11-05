using Maestro.Interceptors;

namespace Maestro.Fluent
{
	internal class TypeInstanceBuilder<TInstance> : ITypeInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public TypeInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public ILifetimeSelector<ITypeInstanceBuilder<TInstance>> Lifetime
		{
			get { return new LifetimeSelector<ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifetime); }
		}

		public ITypeInstanceBuilder<TInstance> InterceptWith(IInterceptor interceptor)
		{
			_pipelineEngine.AddInterceptor(interceptor);
			return this;
		}
	}
}