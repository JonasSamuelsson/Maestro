using Maestro.Interceptors;

namespace Maestro.Fluent
{
	internal class LambdaInstanceBuilder<TInstance> : ILambdaInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public ILifetimeSelector<ILambdaInstanceBuilder<TInstance>> Lifetime
		{
			get { return new LifetimeSelector<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifetime); }
		}

		public ILambdaInstanceBuilder<TInstance> InterceptWith(IInterceptor interceptor)
		{
			_pipelineEngine.AddInterceptor(interceptor);
			return this;
		}
	}
}