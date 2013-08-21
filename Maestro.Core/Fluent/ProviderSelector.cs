using System;

namespace Maestro.Fluent
{
	internal class ProviderSelector : IProviderSelector
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public ProviderSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		public IConstantInstancePipelineBuilder Use(object instance)
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder(provider, pipeline);
		}

		public ILambdaInstanceBuilder<object> Use(Func<object> func)
		{
			return Use(_ => func());
		}

		public ILambdaInstanceBuilder<object> Use(Func<IContext, object> func)
		{
			var provider = new LambdaInstanceProvider(func);
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new LambdaInstanceBuilder<object>(pipeline);
		}

		public ITypeInstanceBuilder<object> Use(Type type)
		{
			var provider = new TypeInstanceProvider(type);
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstanceBuilder<object>(pipeline);
		}
	}

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

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin
		{
			return Use(_ => func());
		}

		public ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin
		{
			var provider = new LambdaInstanceProvider(context => func(context));
			var pipeline = new PipelineEngine(provider);
			_registerPipeline(pipeline);
			return new LambdaInstanceBuilder<TInstance>(pipeline);
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