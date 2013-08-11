﻿using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		IFuncInstancePipelineBuilder As(Func<object> func);
		IFuncInstancePipelineBuilder As(Func<IContext, object> func);
		ITypeInstancePipelineBuilder As(Type type);
	}

	public interface IPipelineSelector<TPlugin>
	{
		IFuncInstancePipelineBuilder<TInstance> As<TInstance>(Func<TInstance> func) where TInstance : TPlugin;
		IFuncInstancePipelineBuilder<TInstance> As<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin;
		ITypeInstancePipelineBuilder<TInstance> As<TInstance>() where TInstance : TPlugin;
	}
}