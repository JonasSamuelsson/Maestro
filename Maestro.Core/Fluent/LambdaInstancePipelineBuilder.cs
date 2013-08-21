namespace Maestro.Fluent
{
	internal class LambdaInstancePipelineBuilder : ILambdaInstancePipelineBuilder
	{
		private readonly LambdaInstanceProvider _provider;
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstancePipelineBuilder(LambdaInstanceProvider provider, IPipelineEngine pipelineEngine)
		{
			_provider = provider;
			_pipelineEngine = pipelineEngine;
		}
	}

	internal class LambdaInstancePipelineBuilder<TInstance> : ILambdaInstancePipelineBuilder<TInstance>
	{
		private readonly LambdaInstanceProvider _provider;
		private readonly IPipelineEngine _pipelineEngine;

		public LambdaInstancePipelineBuilder(LambdaInstanceProvider provider, IPipelineEngine pipelineEngine)
		{
			_provider = provider;
			_pipelineEngine = pipelineEngine;
		}
	}
}