using System;

namespace Maestro.Fluent
{
	internal class PipelineSelector : IProviderSelector
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
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder(provider, pipeline);
		}

		public ILambdaInstancePipelineBuilder Use(Func<object> func)
		{
			return Use(_ => func());
		}

		public ILambdaInstancePipelineBuilder Use(Func<IContext, object> func)
		{
			var provider = new LambdaInstanceProvider(func);
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new LambdaInstancePipelineBuilder(provider, pipeline);
		}

		public ITypeInstanceBuilder Use(Type type)
		{
			var provider = new TypeInstanceProvider(type);
			var pipeline = new PipelineEngine(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstancePipelineBuilder(provider, pipeline);
		}
	}
}