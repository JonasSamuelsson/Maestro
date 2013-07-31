namespace Maestro.Fluent
{
	internal class TypePipelineBuilder : ITypePipelineBuilder
	{
		private readonly IProvider _provider;
		private readonly IPipeline _pipeline;

		public TypePipelineBuilder(IProvider provider, IPipeline pipeline)
		{
			_provider = provider;
			_pipeline = pipeline;
		}
	}
}