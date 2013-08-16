namespace Maestro.Fluent
{
	internal class LambdaInstancePipelineBuilder : ILambdaInstancePipelineBuilder
	{
		private readonly LambdaInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public LambdaInstancePipelineBuilder(LambdaInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}

	internal class LambdaInstancePipelineBuilder<TInstance> : ILambdaInstancePipelineBuilder<TInstance>
	{
		private readonly LambdaInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public LambdaInstancePipelineBuilder(LambdaInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}
}