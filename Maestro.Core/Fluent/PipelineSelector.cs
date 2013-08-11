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

		IFuncInstancePipelineBuilder IDefaultPipelineSelector.Is(Func<object> func)
		{
			return Func(_ => func());
		}

		IFuncInstancePipelineBuilder IDefaultPipelineSelector.Is(Func<IContext, object> func)
		{
			return Func(func);
		}

		IFuncInstancePipelineBuilder IPipelineSelector.As(Func<object> func)
		{
			return Func(_ => func());
		}

		IFuncInstancePipelineBuilder IPipelineSelector.As(Func<IContext, object> func)
		{
			return Func(func);
		}

		private IFuncInstancePipelineBuilder Func(Func<IContext, object> func)
		{
			var provider = new FuncInstanceProvider(func);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new FuncInstancePipelineBuilder(provider, pipeline);
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

		IFuncInstancePipelineBuilder<TInstance> IDefaultPipelineSelector<TPlugin>.Is<TInstance>(Func<TInstance> func)
		{
			return Func(_ => func());
		}

		IFuncInstancePipelineBuilder<TInstance> IDefaultPipelineSelector<TPlugin>.Is<TInstance>(Func<IContext, TInstance> func)
		{
			return Func(func);
		}

		IFuncInstancePipelineBuilder<TInstance> IPipelineSelector<TPlugin>.As<TInstance>(Func<TInstance> func)
		{
			return Func(_ => func());
		}

		IFuncInstancePipelineBuilder<TInstance> IPipelineSelector<TPlugin>.As<TInstance>(Func<IContext, TInstance> func)
		{
			return Func(func);
		}

		private IFuncInstancePipelineBuilder<TInstance> Func<TInstance>(Func<IContext, TInstance> func)
		{
			var provider = new FuncInstanceProvider(context => func(context));
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new FuncInstancePipelineBuilder<TInstance>(provider, pipeline);
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