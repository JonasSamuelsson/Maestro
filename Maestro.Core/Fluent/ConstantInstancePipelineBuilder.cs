namespace Maestro.Fluent
{
	internal class ConstantInstancePipelineBuilder : IConstantInstancePipelineBuilder
	{
		private readonly ConstantInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public ConstantInstancePipelineBuilder(ConstantInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}

	internal class ConstantInstancePipelineBuilder<TInstance> : IConstantInstancePipelineBuilder<TInstance>
	{
		private readonly ConstantInstanceProvider _provider;
		private readonly IPipeline _pipeline;

		public ConstantInstancePipelineBuilder(ConstantInstanceProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}
}