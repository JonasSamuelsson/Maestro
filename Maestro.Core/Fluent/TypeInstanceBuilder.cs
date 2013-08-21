namespace Maestro.Fluent
{
	internal class TypeInstanceBuilder<TInstance> : ITypeInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public TypeInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public ILifecycleSelector<ITypeInstanceBuilder<TInstance>> Lifecycle
		{
			get { return new LifecycleSelector<ITypeInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifecycle); }
		}
	}
}