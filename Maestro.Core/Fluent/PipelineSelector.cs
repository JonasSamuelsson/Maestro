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

		public IConstantInstancePipelineBuilder Is(object instance)
		{
			return Contant(instance);
		}

		public IConstantInstancePipelineBuilder As(object instance)
		{
			return Contant(instance);
		}

		private IConstantInstancePipelineBuilder Contant(object instance)
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder(provider, pipeline);
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

		IConstantInstancePipelineBuilder<TInstance> IDefaultPipelineSelector<TPlugin>.Is<TInstance>(TInstance instance)
		{
			return Constant(instance);
		}

		IConstantInstancePipelineBuilder<TInstance> IPipelineSelector<TPlugin>.As<TInstance>(TInstance instance)
		{
			return Constant(instance);
		}

		private IConstantInstancePipelineBuilder<TInstance> Constant<TInstance>(TInstance instance)
		{
			var provider = new ConstantInstanceProvider(instance);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new ConstantInstancePipelineBuilder<TInstance>(provider, pipeline);
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