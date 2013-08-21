namespace Maestro.Fluent
{
	internal class LambdaInstanceBuilder<TInstance> : ILambdaInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public ILifecycleSelector<ILambdaInstanceBuilder<TInstance>> Lifecycle
		{
			get { return new LifecycleSelector<ILambdaInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifecycle); }
		}
	}
}