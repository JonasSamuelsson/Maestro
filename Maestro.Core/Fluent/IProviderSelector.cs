using System;

namespace Maestro.Fluent
{
	public interface IProviderSelector
	{
		IConstantInstancePipelineBuilder Use(object instance);
		ILambdaInstancePipelineBuilder Use(Func<object> func);
		ILambdaInstancePipelineBuilder Use(Func<IContext, object> func);
		ITypeInstanceBuilder<object> Use(Type type);
	}

	public interface IProviderSelector<TPlugin>
	{
		IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin;
		ILambdaInstancePipelineBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin;
		ITypeInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
		void UseConditional(Action<IConditionalInstancePipelineBuilder<TPlugin>> action);
	}
}