using System;

namespace Maestro.Fluent
{
	public interface IProviderSelector
	{
		IConstantInstancePipelineBuilder Use(object instance);
		ILambdaInstanceBuilder<object> Use(Func<object> func);
		ILambdaInstanceBuilder<object> Use(Func<IContext, object> func);
		ITypeInstanceBuilder<object> Use(Type type);
	}

	public interface IProviderSelector<TPlugin>
	{
		IConstantInstancePipelineBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> func) where TInstance : TPlugin;
		ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> func) where TInstance : TPlugin;
		ITypeInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
		void UseConditional(Action<IConditionalInstancePipelineBuilder<TPlugin>> action);
	}
}