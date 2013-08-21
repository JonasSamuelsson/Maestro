namespace Maestro.Fluent
{
	internal class ConstantInstancePipelineBuilder : IConstantInstancePipelineBuilder
	{
		private readonly ConstantInstanceProvider _provider;
		private readonly IPipelineEngine _pipelineEngine;

		public ConstantInstancePipelineBuilder(ConstantInstanceProvider provider, IPipelineEngine pipelineEngine)
		{
			_provider = provider;
			_pipelineEngine = pipelineEngine;
		}
	}

	internal class ConstantInstancePipelineBuilder<TInstance> : IConstantInstancePipelineBuilder<TInstance>
	{
		private readonly ConstantInstanceProvider _provider;
		private readonly IPipelineEngine _pipelineEngine;

		public ConstantInstancePipelineBuilder(ConstantInstanceProvider provider, IPipelineEngine pipelineEngine)
		{
			_provider = provider;
			_pipelineEngine = pipelineEngine;
		}
	}
}