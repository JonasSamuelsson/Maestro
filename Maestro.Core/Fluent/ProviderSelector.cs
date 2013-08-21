using System;

namespace Maestro.Fluent
{
	internal class ProviderSelector<TPlugin> : IProviderSelector<TPlugin>
	{
		private readonly Action<IPipelineEngine> _registerPipeline;

		public ProviderSelector(Action<IPipelineEngine> registerPipeline)
		{
			_registerPipeline = registerPipeline;
		}

		public IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
			return new ConstantInstancePipelineBuilder<TInstance>(provider, pipeline);
		}

		public ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin
		{
			return Use(_ => func());
		}

		public ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin
		{
			var provider = new LambdaInstanceProvider(context => func(context));
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
			return new LambdaInstancePipelineBuilder<TInstance>(provider, pipeline);
		}

		public ITypeInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			var provider = new TypeInstanceProvider(typeof(TInstance));
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
			return new TypeInstanceBuilder<TInstance>(pipeline);
		}

		public void UseConditional(Action<IConditionalInstancePipelineBuilder<TPlugin>> action)
		{
			var provider = new ConditionalInstanceProvider<TPlugin>(action);
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
		}
	}
}