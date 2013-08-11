namespace Maestro.Fluent
{
	internal class FuncInstancePipelineBuilder : IFuncInstancePipelineBuilder
	{
		private readonly FuncInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public FuncInstancePipelineBuilder(FuncInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}

	internal class FuncInstancePipelineBuilder<TInstance> : IFuncInstancePipelineBuilder<TInstance>
	{
		private readonly FuncInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public FuncInstancePipelineBuilder(FuncInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}
}