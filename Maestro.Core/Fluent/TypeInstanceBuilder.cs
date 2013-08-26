namespace Maestro.Fluent
{
	internal class TypeInstanceBuilder<TInstance> : ITypeInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public TypeInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public IInterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>> OnCreate
		{
			get { return new InterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnCreateInterceptor); }
		}

		public ILifecycleSelector<ITypeInstanceBuilder<TInstance>> Lifecycle
		{
			get { return new LifecycleSelector<ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifecycle); }
		}

		public IInterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>> OnActivate
		{
			get { return new InterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnActivateInterceptor); }
		}
	}
}