using System;

namespace Maestro.Fluent
{
	public interface IDefaultPipelineSelector
	{
		IFuncInstancePipelineBuilder Is(Func<object> func);
		IFuncInstancePipelineBuilder Is(Func<IContext, object> func);
		ITypeInstancePipelineBuilder Is(Type type);
	}

	public interface IDefaultPipelineSelector<TPlugin>
	{
		IFuncInstancePipelineBuilder<TInstance> Is<TInstance>(Func<TInstance> func) where TInstance:TPlugin;
		IFuncInstancePipelineBuilder<TInstance> Is<TInstance>(Func<IContext, TInstance> func) where TInstance:TPlugin;
		ITypeInstancePipelineBuilder<TInstance> Is<TInstance>() where TInstance : TPlugin;
	}
}