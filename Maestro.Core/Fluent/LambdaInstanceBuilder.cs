namespace Maestro.Fluent
{
	internal class LambdaInstanceBuilder<TInstance> : ILambdaInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public IInterceptExpression<ILambdaInstanceBuilder<TInstance>> OnCreate
		{
			get { return new InterceptExpression<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnCreateInterceptor); }
		}

		public ILifecycleSelector<ILambdaInstanceBuilder<TInstance>> Lifecycle
		{
			get { return new LifecycleSelector<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifecycle); }
		}

		public IInterceptExpression<ILambdaInstanceBuilder<TInstance>> OnActivate
		{
			get { return new InterceptExpression<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnActivateInterceptor); }
		}
	}
}