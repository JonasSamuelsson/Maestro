namespace Maestro.Fluent
{
	internal class LambdaInstanceBuilder<TInstance> : ILambdaInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public IInterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>> OnCreate
		{
			get { return new InterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnCreateInterceptor); }
		}

		public ILifetimeSelector<ILambdaInstanceBuilder<TInstance>> Lifetime
		{
			get { return new LifetimeSelector<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifetime); }
		}

		public IInterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>> OnActivate
		{
			get { return new InterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnActivateInterceptor); }
		}
	}
}