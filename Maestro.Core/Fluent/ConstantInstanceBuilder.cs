namespace Maestro.Fluent
{
	internal class ConstantInstanceBuilder<TInstance> : IConstantInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public ConstantInstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}
	}
}