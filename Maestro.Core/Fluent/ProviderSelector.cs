using System;

namespace Maestro.Fluent
{
	internal class ProviderSelector<TPlugin> : IProviderSelector<TPlugin>
	{
		private readonly Action<IPipelineEngine> _registerPipeline;
		private readonly DefaultSettings _defaultSettings;

		public ProviderSelector(Action<IPipelineEngine> registerPipeline, DefaultSettings defaultSettings)
		{
			_registerPipeline = registerPipeline;
			_defaultSettings = defaultSettings;
		}

		public IConstantInstanceBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
			return new ConstantInstanceBuilder<TInstance>(pipeline);
		}

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin
		{
			return Use(_ => func());
		}

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin
		{
			var provider = new LambdaInstanceProvider(context => func(context));
			var pipeline = new PipelineEngine(provider);
			pipeline.SetLifetime(_defaultSettings.GetLifetime());
			_registerPipeline(pipeline);
			return new LambdaInstanceBuilder<TInstance>(pipeline);
		}

		public ITypeInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			return UseType<TInstance>(typeof(TInstance));
		}

		public ITypeInstanceBuilder<TPlugin> Use(Type type)
		{
			return UseType<TPlugin>(type);
		}

		private ITypeInstanceBuilder<T> UseType<T>(Type type)
		{
			var provider = new TypeInstanceProvider(type);
			var pipeline = new PipelineEngine(provider);
			pipeline.SetLifetime(_defaultSettings.GetLifetime());
			_registerPipeline(pipeline);
			return new TypeInstanceBuilder<T>(pipeline);
		}

		public void UseConditional(Action<IConditionalInstanceBuilder<TPlugin>> action)
		{
			var builder = new ConditionalInstanceBuilder<TPlugin>(_defaultSettings);
			var pipeline = builder.GetPipeline(action);
			_registerPipeline(pipeline);
		}
	}
}