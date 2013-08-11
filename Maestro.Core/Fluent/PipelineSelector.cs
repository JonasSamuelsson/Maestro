using System;

namespace Maestro.Fluent
{
	internal class PipelineSelector : IDefaultPipelineSelector, IPipelineSelector
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public PipelineSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		ITypeInstancePipelineBuilder IDefaultPipelineSelector.Is(Type type)
		{
			return Type(type);
		}

		ITypeInstancePipelineBuilder IPipelineSelector.As(Type type)
		{
			return Type(type);
		}

		private ITypeInstancePipelineBuilder Type(Type type)
		{
			var provider = new TypeInstanceProvider(type);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstancePipelineBuilder(provider, pipeline);
		}
	}

	internal class PipelineSelector<TPlugin> : IDefaultPipelineSelector<TPlugin>, IPipelineSelector<TPlugin>
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public PipelineSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		ITypeInstancePipelineBuilder<TInstance> IDefaultPipelineSelector<TPlugin>.Is<TInstance>()
		{
			return Type<TInstance>();
		}

		ITypeInstancePipelineBuilder<TInstance> IPipelineSelector<TPlugin>.As<TInstance>()
		{
			return Type<TInstance>();
		}

		private ITypeInstancePipelineBuilder<TInstance> Type<TInstance>() where TInstance : TPlugin
		{
			var provider = new TypeInstanceProvider(typeof(TInstance));
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new TypeInstancePipelineBuilder<TInstance>(provider, pipeline);
		}
	}
}