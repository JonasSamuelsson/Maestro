using System;

namespace Maestro.Fluent
{
	internal class PipelineSelector : IPipelineSelector
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public PipelineSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		public IConstantInstancePipelineBuilder Use(object instance)
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder(provider, pipeline);
		}

		public IFuncInstancePipelineBuilder Use(Func<object> func)
		{
			return Use(_ => func());
		}

		public IFuncInstancePipelineBuilder Use(Func<IContext, object> func)
		{
			var provider = new FuncInstanceProvider(func);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new FuncInstancePipelineBuilder(provider, pipeline);
		}

		public ITypeInstancePipelineBuilder Use(Type type)
		{
			var provider = new TypeInstanceProvider(type);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstancePipelineBuilder(provider, pipeline);
		}
	}

	internal class PipelineSelector<TPlugin> : IPipelineSelector<TPlugin>
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public PipelineSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		public IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder<TInstance>(provider, pipeline);
		}

		public IFuncInstancePipelineBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin
		{
			return Use(_ => func());
		}

		public IFuncInstancePipelineBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin
		{
			var provider = new FuncInstanceProvider(context => func(context));
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new FuncInstancePipelineBuilder<TInstance>(provider, pipeline);
		}

		public ITypeInstancePipelineBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			var provider = new TypeInstanceProvider(typeof(TInstance));
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstancePipelineBuilder<TInstance>(provider, pipeline);
		}
	}
}