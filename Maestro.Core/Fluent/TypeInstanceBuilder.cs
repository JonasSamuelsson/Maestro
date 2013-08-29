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

		public ILifetimeSelector<ITypeInstanceBuilder<TInstance>> Lifetime
		{
			get { return new LifetimeSelector<ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifetime); }
		}

		public IInterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>> OnActivate
		{
			get { return new InterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.AddOnActivateInterceptor); }
		}
	}
}