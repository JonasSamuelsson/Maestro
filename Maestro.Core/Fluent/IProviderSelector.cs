using System;

namespace Maestro.Fluent
{
	public interface IProviderSelector<TPlugin>
	{
		IConstantInstanceBuilder<TInstance> Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin;
		ILambdaInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin;
		ITypeInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
		ITypeInstanceBuilder<TPlugin> Use(Type type);
		void UseConditional(Action<IConditionalInstanceBuilder<TPlugin>> action);
	}
}