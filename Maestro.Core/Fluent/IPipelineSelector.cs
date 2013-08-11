using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		IConstantInstancePipelineBuilder Use(object instance);
		IFuncInstancePipelineBuilder Use(Func<object> func);
		IFuncInstancePipelineBuilder Use(Func<IContext, object> func);
		ITypeInstancePipelineBuilder Use(Type type);
	}

	public interface IPipelineSelector<TPlugin>
	{
		IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		IFuncInstancePipelineBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin;
		IFuncInstancePipelineBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin;
		ITypeInstancePipelineBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
	}
}