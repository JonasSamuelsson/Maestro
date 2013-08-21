namespace Maestro.Fluent
{
	internal class TypeInstancePipelineBuilder : ITypeInstanceBuilder
	{
		private readonly IProvider _provider;
		private readonly IPipelineEngine _pipelineEngine;

		public TypeInstancePipelineBuilder(IProvider provider, IPipelineEngine pipelineEngine)
		{
			_provider = provider;
			_pipelineEngine = pipelineEngine;
		}
	}
}