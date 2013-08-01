namespace Maestro.Fluent
{
	internal class TypeInstancePipelineBuilder : ITypeInstancePipelineBuilder
	{
		private readonly IProvider _provider;
		private readonly IPipeline _pipeline;

		public TypeInstancePipelineBuilder(IProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}

	internal class TypeInstancePipelineBuilder<TInstance> : ITypeInstancePipelineBuilder<TInstance>
	{
		private readonly IProvider _provider;
		private readonly IPipeline _pipeline;

		public TypeInstancePipelineBuilder(IProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}
}