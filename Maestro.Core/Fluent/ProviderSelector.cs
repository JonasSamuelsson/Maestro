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

		public void Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			 //ReSharper disable once CompareNonConstrainedGenericWithNull
			if (instance == null) throw new ArgumentNullException("instance");
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
		}

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			return Use(_ => lambda());
		}

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			var provider = new LambdaInstanceProvider(context => lambda(context));
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
			if (type == null) throw new ArgumentNullException();
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
			if (action == null) throw new ArgumentNullException();
			var builder = new ConditionalInstanceBuilder<TPlugin>(_defaultSettings);
			var pipeline = builder.GetPipeline(action);
			_registerPipeline(pipeline);
		}
	}
}