using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		IConstantInstancePipelineBuilder Use(object instance);
		ILambdaInstancePipelineBuilder Use(Func<object> func);
		ILambdaInstancePipelineBuilder Use(Func<IContext, object> func);
		ITypeInstancePipelineBuilder Use(Type type);
	}

	public interface IPipelineSelector<TPlugin>
	{
		IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin;
		ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin;
		ITypeInstancePipelineBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
		void UseConditional(Action<IConditionalInstancePipelineBuilder<TPlugin>> action);
	}
}